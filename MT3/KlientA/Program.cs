using MassTransit;
using Wiadomosci;

namespace KlientA
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Klient A");

            bool wTrakcieZamowienia = false;

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                cfg.ReceiveEndpoint("klient_a_queue", ep =>
                {
                    ep.Handler<PytanieoPotwierdzenie>(async ctx =>
                    {
                        wTrakcieZamowienia = true;

                        Console.WriteLine($"\nZapytanie o potwierdzenie: Ilość = {ctx.Message.Ilosc}");

                        Console.WriteLine("Naciśnij [T] by potwierdzić lub [N] by odrzucić:");
                        var key = Console.ReadKey().Key;

                        if (key == ConsoleKey.T)
                        {
                            await ctx.RespondAsync(new Potwierdzenie(ctx.Message.OrderId));
                            Console.WriteLine("\nWysłano potwierdzenie");
                        }
                        else
                        {
                            await ctx.RespondAsync(new BrakPotwierdzenia(ctx.Message.OrderId));
                            Console.WriteLine("\nWysłano brak potwierdzenia");
                        }
                    });

                    ep.Handler<AkceptacjaZamowienia>(ctx =>
                    {
                        Console.WriteLine($"\nZamówienie zaakceptowane! Ilość: {ctx.Message.Ilosc}");
                        wTrakcieZamowienia = false;
                        return Task.CompletedTask;
                    });

                    ep.Handler<OdrzucenieZamowienia>(ctx =>
                    {
                        Console.WriteLine($"\nZamówienie odrzucone. Ilość: {ctx.Message.Ilosc}");
                        wTrakcieZamowienia = false;
                        return Task.CompletedTask;
                    });
                });
            });

            await bus.StartAsync();

            while (true)
            {
                if (!wTrakcieZamowienia)
                {
                    Console.Write("\nPodaj ilość do zamówienia (lub puste by zakończyć): ");
                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input)) break;
                    wTrakcieZamowienia = true;
                    if (int.TryParse(input, out int ilosc))
                    {
                        var id = Guid.NewGuid();
                        await bus.Publish(new StartZamowienia(id, ilosc, new Uri("queue:klient_a_queue")));
                        Console.WriteLine($"Wysłano zamówienie ID: {id}, ilość: {ilosc}");
                    }
                    else
                    {
                        Console.WriteLine("Błędna liczba.");
                    }
                }
            }

            await bus.StopAsync();
        }
    }
}