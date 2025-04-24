using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_Producer
{
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
            using (IConnection connection = await factory.CreateConnectionAsync())
            using (IChannel channel = await connection.CreateChannelAsync())
            {
                await channel.QueueDeclareAsync("message_queue", true, false, false, null);
                var replyQueue = await channel.QueueDeclareAsync("", false, true, true, null);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, e) =>
                {
                    string response = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine($"Odpowiedź odebrana: {response}");
                    await Task.CompletedTask;
                };
                await channel.BasicConsumeAsync(replyQueue.QueueName, true, consumer);

                for (int i = 1; i <= 10; i++)
                {
                    string message = $"Wiadomość numer {i}";
                    var body = Encoding.UTF8.GetBytes(message);

                    BasicProperties properties = new BasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("Nagłówek 1", 1);
                    properties.Headers.Add("Nagłówek 2", 2);
                    properties.DeliveryMode = DeliveryModes.Persistent;
                    properties.ReplyTo = replyQueue.QueueName;
                    await channel.BasicPublishAsync(string.Empty, "message_queue", false, properties, body);
                    Console.WriteLine($"Wysłano: {message}");
                }

                Console.ReadKey();
            }

        }
    }
}
