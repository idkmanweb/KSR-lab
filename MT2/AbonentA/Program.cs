using System;
using System.Threading.Tasks;
using MassTransit;
using Komunikaty;

namespace AbonentA
{
    public class PublConsumer : IConsumer<IPubl>
    {
        public async Task Consume(ConsumeContext<IPubl> context)
        {
            Console.WriteLine($"Otrzymano: Publ #{context.Message.Numer}");

            if (context.Message.Numer % 2 == 0)
            {
                await context.RespondAsync<IOdpA>(new { Kto = "abonent A" });
                Console.WriteLine("Wysłano OdpA");
            }
        }
    }

    class Program
    {
        public static Task HndlFault(ConsumeContext<Fault<IOdpA>> ctx)
        {
            Console.WriteLine("Abonent A: OdpA wywołała wyjątek");
            foreach (var e in ctx.Message.Exceptions)
            {
                Console.WriteLine($"- {e.Message}");
            }
            return Task.CompletedTask;
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Abonent A");

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                cfg.ReceiveEndpoint("abonent_a", ep =>
                {
                    ep.Consumer<PublConsumer>();
                    ep.Handler<Fault<IOdpA>>(HndlFault);
                });
            });

            await bus.StartAsync();
            Console.WriteLine("Abonent A uruchomiony. Oczekiwanie na wiadomości...");

            Console.ReadLine();
            await bus.StopAsync();
        }
    }
}
