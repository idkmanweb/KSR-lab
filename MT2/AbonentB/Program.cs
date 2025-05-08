using System;
using System.Threading.Tasks;
using MassTransit;
using Komunikaty;

namespace AbonentB
{
    public class PublConsumer : IConsumer<IPubl>
    {
        public async Task Consume(ConsumeContext<IPubl> context)
        {
            Console.WriteLine($"Otrzymano: Publ #{context.Message.Numer}");

            if (context.Message.Numer % 3 == 0)
            {
                Console.WriteLine("Wysłano OdpB");
                await context.RespondAsync<IOdpB>(new { Kto = "abonent B" });
            }
        }
    }

    class Program
    {
        public static Task HndlFault(ConsumeContext<Fault<IOdpB>> ctx)
        {
            Console.WriteLine("Abonent B: OdpB wywołała wyjątek");
            foreach (var e in ctx.Message.Exceptions)
            {
                Console.WriteLine($"- {e.Message}");
            }
            return Task.CompletedTask;
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Abonent B");

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                cfg.ReceiveEndpoint("abonent_b", ep =>
                {
                    ep.Consumer<PublConsumer>();
                    ep.Handler<Fault<IOdpB>>(HndlFault);
                });
            });

            await bus.StartAsync();
            Console.WriteLine("Abonent B uruchomiony. Oczekiwanie na wiadomości...");

            Console.ReadLine();
            await bus.StopAsync(); 
        }
    }
}
