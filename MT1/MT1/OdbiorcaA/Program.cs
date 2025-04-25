using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Komunikaty;

namespace OdbiorcaA
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Odbiorca A");
            Console.WriteLine();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                sbc.ReceiveEndpoint("mta_queue", ep =>
                {
                    ep.Handler<Komunikaty.IKomunikat>(context =>
                    {
                        Console.WriteLine($"Odebrano: {context.Message.text}");

                        foreach (var hdr in context.Headers.GetAll())
                        {
                            Console.WriteLine("{0}: {1}", hdr.Key, hdr.Value);
                        }

                        return Task.CompletedTask;
                    });

                });
            });

            await bus.StartAsync();

            Console.ReadLine();

            await bus.StopAsync();
        }
    }
}
