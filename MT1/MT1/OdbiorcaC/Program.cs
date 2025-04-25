using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Komunikaty;

namespace OdbiorcaC
{
    public class OdbiorcaC : IConsumer<IKomunikat2>
    {
        public Task Consume(ConsumeContext<IKomunikat2> context)
        {
            Console.WriteLine($"Odebrano: {context.Message.text}");
            return Task.CompletedTask;
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Odbiorca C");
            Console.WriteLine();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                sbc.ReceiveEndpoint("mtc_queue", ep =>
                {
                    ep.Consumer<OdbiorcaC>();
                });
            });

            await bus.StartAsync();

            Console.ReadLine();

            await bus.StopAsync();
        }
    }
}
