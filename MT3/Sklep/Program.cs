using MassTransit;
using Wiadomosci;
using MassTransit.QuartzIntegration;

namespace Sklep
{
    public class ZamowienieSagaDane : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        public int Ilosc { get; set; }

        public bool? PotwierdzenieKlienta { get; set; }
        public bool? PotwierdzenieMagazynu { get; set; }

        public Guid? TimeoutId { get; set; }
        public Uri ClientQueue { get; set; }
    }
    public class ZamowienieSaga : MassTransitStateMachine<ZamowienieSagaDane>
    {
        public State Oczekiwanie { get; private set; }
        public State Zakonczone { get; private set; }

        public Event<StartZamowienia> StartZamowieniaEvt { get; private set; }
        public Event<OdpowiedzWolne> MagazynOkEvt { get; private set; }
        public Event<OdpowiedzWolneNegatywna> MagazynNieEvt { get; private set; }
        public Event<Potwierdzenie> KlientOkEvt { get; private set; }
        public Event<BrakPotwierdzenia> KlientNieEvt { get; private set; }

        public Schedule<ZamowienieSagaDane, Wiadomosci.Timeout> TO { get; private set; }

        public ZamowienieSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => StartZamowieniaEvt, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => MagazynOkEvt, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => MagazynNieEvt, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => KlientOkEvt, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => KlientNieEvt, x => x.CorrelateById(m => m.Message.OrderId));

            Schedule(() => TO, x => x.TimeoutId, s =>
            {
                s.Delay = TimeSpan.FromSeconds(10);
                s.Received = e => e.CorrelateById(x => x.Message.CorrelationId);
            });

            Initially(
                When(StartZamowieniaEvt)
                    .Then(ctx =>
                    {
                        ctx.Instance.Ilosc = ctx.Data.Ilosc;
                        ctx.Instance.ClientQueue = ctx.Data.ClientQueue;
                        ctx.Instance.PotwierdzenieKlienta = null;
                        ctx.Instance.PotwierdzenieMagazynu = null;
                    })
                    .ThenAsync(async ctx =>
                    {
                        await ctx.Send(ctx.Instance.ClientQueue, new PytanieoPotwierdzenie(ctx.Data.OrderId, ctx.Data.Ilosc));
                        await ctx.Publish(new PytanieoWolne(ctx.Data.OrderId, ctx.Data.Ilosc));
                    })
                    .Schedule(TO, ctx => new Wiadomosci.Timeout { CorrelationId = ctx.Data.OrderId })
                    .TransitionTo(Oczekiwanie)
            );

            During(Oczekiwanie,
                When(KlientOkEvt)
                    .Then(ctx => {
                        ctx.Instance.PotwierdzenieKlienta = true;
                    })
                    .ThenAsync(SprawdzKoniec),
                When(KlientNieEvt)
                    .Then(ctx => {
                        ctx.Instance.PotwierdzenieKlienta = false;
                    })
                    .ThenAsync(SprawdzKoniec),

                When(MagazynNieEvt)
                    .Then(ctx => ctx.Instance.PotwierdzenieMagazynu = false)
                    .ThenAsync(SprawdzKoniec),
                When(MagazynOkEvt)
                    .Then(ctx => ctx.Instance.PotwierdzenieMagazynu = true)
                    .ThenAsync(SprawdzKoniec),

                When(TO.Received)
                .ThenAsync(async ctx =>
                {
                    ctx.Instance.PotwierdzenieKlienta = false;
                    ctx.Instance.PotwierdzenieMagazynu = false;
                    Console.WriteLine($"Timeout: Odrzucanie zamówienia {ctx.Instance.CorrelationId}");
                    await ctx.Send(new Uri("queue:magazyn_queue"), new OdrzucenieZamowienia(ctx.Instance.CorrelationId, ctx.Instance.Ilosc));
                    await ctx.Send(ctx.Instance.ClientQueue, new OdrzucenieZamowienia(ctx.Instance.CorrelationId, ctx.Instance.Ilosc));

                })
                .Finalize()
            );

            SetCompletedWhenFinalized();
        }

        private async Task SprawdzKoniec(BehaviorContext<ZamowienieSagaDane> ctx)
        {
            var i = ctx.Instance;
            if (i.PotwierdzenieMagazynu.HasValue && i.PotwierdzenieKlienta.HasValue)
            {
                if (i.PotwierdzenieMagazynu.Value && i.PotwierdzenieKlienta.Value)
                {
                    Console.WriteLine($"Potwierdzam zamówienie {i.CorrelationId} o ilość {i.Ilosc}");
                    await ctx.Send(new Uri("queue:magazyn_queue"), new AkceptacjaZamowienia(ctx.Instance.CorrelationId, ctx.Instance.Ilosc));
                    await ctx.Send(ctx.Instance.ClientQueue, new AkceptacjaZamowienia(i.CorrelationId, i.Ilosc));
                    ctx.Instance.CurrentState = Zakonczone.Name;
                    MarkCompleted(ctx);
                }
                else if (!i.PotwierdzenieMagazynu.Value || !i.PotwierdzenieKlienta.Value)
                {
                    Console.WriteLine($"Odrzucam zamówienie {i.CorrelationId} o ilość {i.Ilosc} - magazyn: {i.PotwierdzenieMagazynu.Value}, klient: {i.PotwierdzenieKlienta.Value}");
                    await ctx.Send(new Uri("queue:magazyn_queue"), new OdrzucenieZamowienia(ctx.Instance.CorrelationId, ctx.Instance.Ilosc));
                    await ctx.Send(ctx.Instance.ClientQueue, new OdrzucenieZamowienia(i.CorrelationId, i.Ilosc));
                    ctx.Instance.CurrentState = Zakonczone.Name;
                    MarkCompleted(ctx);
                }
            }
        }

        private void MarkCompleted(BehaviorContext<ZamowienieSagaDane> ctx)
        {
            ctx.Instance.CurrentState = Zakonczone.Name;
        }
    }
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Sklep");

            var saga = new ZamowienieSaga();
            var repo = new InMemorySagaRepository<ZamowienieSagaDane>();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                sbc.UseInMemoryScheduler();

                sbc.ReceiveEndpoint("zamowienie_saga", ep =>
                {
                    ep.StateMachineSaga(saga, repo);
                });
            });

            await bus.StartAsync();
            Console.ReadLine();
            await bus.StopAsync();
        }
    }
}