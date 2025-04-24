using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RMQ_Consumer2
{
    class MyConsumer : AsyncDefaultBasicConsumer
    {
        public MyConsumer(IChannel channel) : base(channel) { }
        public override async Task HandleBasicDeliverAsync(string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IReadOnlyBasicProperties properties,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
        {
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine($"Otrzymano: {message}");
            if (properties.Headers != null)
            {
                foreach (var header in properties.Headers)
                {
                    var value = header.Value is byte[] bytes ? Encoding.UTF8.GetString(bytes) : header.Value?.ToString();

                    Console.WriteLine($"Nagłówek: {header.Key} = {value}");
                }
            }
            await Task.Delay(2000);

            string responseMessage = $"Odpowiedź na: {message}";
            var responseBody = Encoding.UTF8.GetBytes(responseMessage);

            var responseProperties = new BasicProperties
            {
                ReplyTo = properties.ReplyTo
            };

            await Channel.BasicPublishAsync(string.Empty, properties.ReplyTo, false, responseProperties, responseBody);
            Console.WriteLine($"Odpowiedziano: {responseMessage}");

            Console.WriteLine($"Potwierdzono: {message}");
            await Channel.BasicAckAsync(deliveryTag, multiple: false);
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "ugawydjd",
                Password = "ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P",
                HostName = "hawk.rmq.cloudamqp.com",
                VirtualHost = "ugawydjd"
            };

            Console.WriteLine("Consumer 2");
            Console.WriteLine();

            using (IConnection connection = await factory.CreateConnectionAsync())
            using (IChannel channel = await connection.CreateChannelAsync())
            {
                await channel.BasicQosAsync(0, 1, false);
                AsyncDefaultBasicConsumer consumer = new MyConsumer(channel);
                await channel.BasicConsumeAsync("message_queue", false, consumer);

                Console.ReadKey();
            }

        }
    }
}
