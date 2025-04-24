using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

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
                await channel.ExchangeDeclareAsync("top_rout", "topic");

                await Task.Delay(500);

                for (int i = 1; i <= 10; i++)
                {
                    string message = $"Wiadomość numer {i}";
                    var body = Encoding.UTF8.GetBytes(message);

                    string routingKey = i % 2 == 0 ? "abc.def" : "abc.xyz";
                    await channel.BasicPublishAsync(
                        exchange: "top_rout",
                        routingKey: routingKey,
                        mandatory: false,
                        body: body
                    );

                    Console.WriteLine($"Wysłano: {message} na {routingKey}");

                    await Task.Delay(100);
                }

                Console.ReadKey();
            }
        }
    }
}
