using MassTransit;
using Wiadomosci;

namespace Magazyn
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            int wolne = 0;
            int zarezerwowane = 0;
            bool zaakceptowano = false;

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                cfg.ReceiveEndpoint("magazyn_queue", ep =>
                {
                    ep.Handler<PytanieoWolne>(async context =>
                    {
                        var ilosc = context.Message.Ilosc;
                        var orderId = context.Message.OrderId;

                        Console.WriteLine($"\nZamówienie {orderId} - zapytanie o {ilosc} sztuk");

                        if (wolne >= ilosc)
                        {
                            wolne -= ilosc;
                            zarezerwowane += ilosc;
                            zaakceptowano = true;
                            Console.WriteLine($"Potwierdzenie - Zarezerwowano {ilosc} sztuk (Zamówienie {orderId})");
                            await context.Publish(new OdpowiedzWolne(orderId));
                            PokazStan();
                        }
                        else
                        {
                            zaakceptowano = false;
                            Console.WriteLine($"Brak dostępności - Wolne: {wolne}, potrzebne: {ilosc} (Zamówienie {orderId})");
                            await context.Publish(new OdpowiedzWolneNegatywna(orderId));
                        }

                    });

                    ep.Handler<AkceptacjaZamowienia>(async context =>
                    {
                        var ilosc = context.Message.Ilosc;
                        zarezerwowane -= ilosc;
                        zaakceptowano = false;
                        Console.WriteLine($"Akceptacja Zamówienia {context.Message.OrderId}, usunięto {ilosc} z magazynu");
                        PokazStan();
                        Console.Write("\nPodaj liczbę do dodania do magazynu (lub ENTER, by pominąć): ");
                    });

                    ep.Handler<OdrzucenieZamowienia>(async context =>
                    {
                        var ilosc = context.Message.Ilosc;
                        if (zaakceptowano)
                        {
                            wolne += ilosc;
                            zarezerwowane -= ilosc;
                            zaakceptowano = false;
                        }
                        Console.WriteLine($"Odrzucenie Zamówienia {context.Message.OrderId}");
                        PokazStan();
                        Console.Write("\nPodaj liczbę do dodania do magazynu (lub ENTER, by pominąć): ");
                    });
                });
            });

            await bus.StartAsync();
            Console.WriteLine("Magazyn uruchomiony.");
            PokazStan();

            while (true)
            {
                Console.Write("\nPodaj liczbę do dodania do magazynu (lub ENTER, by pominąć): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int nowaIlosc) && nowaIlosc > 0)
                {
                    wolne += nowaIlosc;
                    Console.WriteLine($"Dodano {nowaIlosc} sztuk do magazynu.");
                    PokazStan();
                }
            }

            await bus.StopAsync();

            void PokazStan()
            {
                Console.WriteLine($"\nStan magazynu: Wolne = {wolne}, Zarezerwowane = {zarezerwowane}");
            }
        }
    }
}
