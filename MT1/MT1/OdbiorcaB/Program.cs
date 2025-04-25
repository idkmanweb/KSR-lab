using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Komunikaty;
using MassTransit;

namespace OdbiorcaB
{
    public class OdbiorcaB : IConsumer<Komunikat3>, IConsumer<IKomunikat2>, IConsumer<IKomunikat>
    {
        private int counter = 0;
        private int message3received = 0;

        public Task Consume(ConsumeContext<Komunikat3> context)
        {
            counter++;
            Console.WriteLine($"Otrzymano: {context.Message.text}");
            Console.WriteLine($"Licznik wiadomości: {counter}");
            message3received += 2;
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<IKomunikat2> context)
        {
            if (message3received > 0)
            {
                message3received--;
            }
            else
            {
                counter++;
                Console.WriteLine($"Otrzymano: {context.Message.text}");
                Console.WriteLine($"Licznik wiadomości: {counter}");
            }
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<IKomunikat> context)
        {
            if (message3received > 0)
            {
                message3received--;
            }
            else
            {
                counter++;
                Console.WriteLine($"Otrzymano: {context.Message.text}");
                Console.WriteLine($"Licznik wiadomości: {counter}");
            }
            return Task.CompletedTask;
        }
    }



    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Odbiorca B");
            Console.WriteLine();

            var consumer = new OdbiorcaB();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                sbc.ReceiveEndpoint("mtb_queue", ep =>
                {
                    ep.Instance(consumer);
                });
            });

            await bus.StartAsync();

            Console.ReadLine();

            await bus.StopAsync();
        }
    }
}
