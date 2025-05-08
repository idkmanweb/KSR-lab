using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Komunikaty;
using System.Collections.Generic;
using MassTransit.Serialization;
using System.Text;
using MassTransit.Serialization;

namespace Wydawca
{
    public class Klucz : SymmetricKey
    {
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }
    }

    public class Dostawca : ISymmetricKeyProvider
    {
        private string k;
        public Dostawca(string _k) { k = _k; }
        public bool TryGetKey(string keyId, out SymmetricKey key)
        {
            var sk = new Klucz(); 
            sk.IV = Encoding.ASCII.GetBytes(keyId.Substring(0, 16));
            sk.Key = Encoding.ASCII.GetBytes(k);
            key = sk;
            return true;
        }
    }

    public class OdpAConsumer : IConsumer<IOdpA>
    {
        private static readonly Random rand = new Random();

        public Task Consume(ConsumeContext<IOdpA> context)
        {
            Console.WriteLine($"OdpA od: {context.Message.Kto}");

            if (rand.Next(3) == 0)
            {
                Console.WriteLine("Wyjątek dla OdpA");
                context.RespondAsync<Fault<IOdpA>>(new
                {
                    Message = context.Message,
                    Exceptions = new[]
                    {
                        new
                        {
                            ExceptionType = typeof(Exception),
                            Message = "OdpAConsumer Exception"
                        }
                    }
                });
                throw new Exception("OdpAConsumer Exception");
            }

            return Task.CompletedTask;
        }
    }

    public class OdpBConsumer : IConsumer<IOdpB>
    {
        private static readonly Random rand = new Random();

        public Task Consume(ConsumeContext<IOdpB> context)
        {
            Console.WriteLine($"OdpB od: {context.Message.Kto}");

            if (rand.Next(3) == 0)
            {
                Console.WriteLine("Wyjątek dla OdpB");
                context.RespondAsync<Fault<IOdpB>>(new
                {
                    Message = context.Message,
                    Exceptions = new[]
                    {
                        new
                        {
                            ExceptionType = typeof(Exception),
                            Message = "OdpBConsumer Exception"
                        }
                    }
                });
                throw new Exception("OdpBConsumer Exception");
            }

            return Task.CompletedTask;
        }
    }

    public class UstawConsumer : IConsumer<IUstaw>
    {
        public Task Consume(ConsumeContext<IUstaw> context)
        {
            Program.dziala = context.Message.Dziala;
            Console.WriteLine($"Otrzymano polecenie: dziala = {Program.dziala.ToString().ToLower()}");
            return Task.CompletedTask;
        }
    }

    public class ConsumeObserver : IConsumeObserver
    {
        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            Program.Inkrementuj(Program.proby, typeof(T).Name);
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            Program.Inkrementuj(Program.sukcesy, typeof(T).Name);
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }
    }

    public class PublishObserver : IPublishObserver
    {
        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            Program.Inkrementuj(Program.opublikowane, typeof(T).Name);
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }
    }

    class Program
    {
        public static bool dziala = false;

        public static Dictionary<string, int> proby = new Dictionary<string, int>();
        public static Dictionary<string, int> sukcesy = new Dictionary<string, int>();
        public static Dictionary<string, int> opublikowane = new Dictionary<string, int>();

        public static object lockObj = new object();

        public static void Inkrementuj(Dictionary<string, int> dict, string typ)
        {
            lock (lockObj)
            {
                if (!dict.ContainsKey(typ))
                    dict[typ] = 0;
                dict[typ]++;
            }
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Wydawca");

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                sbc.ReceiveEndpoint("kontroler", ep =>
                {
                    ep.Consumer<UstawConsumer>(cfg =>
                    {
                        ep.UseEncryptedSerializer(new AesCryptoStreamProvider(new Dostawca("19338219338219338219338219338219"), "1933821933821933"));
                    });
                });

                sbc.ReceiveEndpoint("wydawca", ep =>
                {

                    ep.Consumer<OdpAConsumer>(cfg =>
                    {
                        cfg.UseRetry(r => r.Interval(5, TimeSpan.FromMilliseconds(100)));
                    });

                    ep.Consumer<OdpBConsumer>(cfg =>
                    {
                        cfg.UseRetry(r => r.Interval(5, TimeSpan.FromMilliseconds(100)));
                    });

                });

            });

            await bus.StartAsync();
            bus.ConnectConsumeObserver(new ConsumeObserver());
            bus.ConnectPublishObserver(new PublishObserver());

            Task.Run(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.S)
                    {
                        Console.WriteLine("\n===== STATYSTYKI =====");

                        lock (lockObj)
                        {
                            Console.WriteLine("-- Próby:");
                            foreach (var kvp in proby)
                                Console.WriteLine($"{kvp.Key}: {kvp.Value}");

                            Console.WriteLine("-- Sukcesy:");
                            foreach (var kvp in sukcesy)
                                Console.WriteLine($"{kvp.Key}: {kvp.Value}");

                            Console.WriteLine("-- Opublikowane:");
                            foreach (var kvp in opublikowane)
                                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                        }

                        Console.WriteLine("======================\n");
                    }
                }
            });

            try
            {
                int i = 1;
                while (true)
                {
                    if (dziala)
                    {
                        var message = new { Numer = i++ };
                        await bus.Publish<IPubl>(message);
                        Console.WriteLine($"Wysłano Publ #{message.Numer}");
                        await Task.Delay(1000);
                    }
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }
}
