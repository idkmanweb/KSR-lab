using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQ_Consumer1
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
                await channel.ExchangeDeclareAsync("top_rout", "topic");

                var queueName = await channel.QueueDeclareAsync();
                await channel.QueueBindAsync(queueName, "top_rout", "abc.*");

                Console.WriteLine("Subscriber 1");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += (sender, e) => {
                    var message = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine($"Otrzymano: {message} z kanału {e.RoutingKey}");
                    return Task.CompletedTask;
                };

                await channel.BasicConsumeAsync(queueName, true, consumer);

                Console.ReadKey();
            }
        }
    }
}
