using System;
using System.Threading.Tasks;
using MassTransit;
using Komunikaty;
using System.Runtime.Remoting.Messaging;

namespace Wydawca
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Wydawca");
            Console.WriteLine();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });
            });

            await bus.StartAsync();

            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    var message = new
                    {
                        text = $"Wiadomość typu 1: #{i}"
                    };

                    await bus.Publish<IKomunikat>(message, context =>
                    {
                        context.Headers.Set("Nagłówek 1", "1");
                        context.Headers.Set("Nagłówek 2", "2");
                    });

                    Console.WriteLine($"Wysłano: {message.text}");

                    await Task.Delay(200);

                    var message2 = new
                    {
                        text = $"Wiadomość typu 2: #{i}"
                    };

                    await bus.Publish<IKomunikat2>(message2);

                    Console.WriteLine($"Wysłano: {message2.text}");

                    await Task.Delay(200);

                    var message3 = new Komunikat3
                    {
                        text = $"Wiadomość typu 3: #{i}"
                    };

                    await bus.Publish<Komunikat3>(message3);

                    Console.WriteLine($"Wysłano: {message3.text}");

                    await Task.Delay(200);
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }
}
