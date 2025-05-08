using System;
using System.Threading.Tasks;
using MassTransit;
using Komunikaty;
using MassTransit.Serialization;
using System.Text;

namespace Kontroler
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
            var sk = new Klucz(); sk.IV = Encoding.ASCII.GetBytes(keyId.Substring(0, 16)); 
            sk.Key = Encoding.ASCII.GetBytes(k); 
            key = sk; 
            return true;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Kontroler");

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd"), h =>
                {
                    h.Username("ugawydjd");
                    h.Password("ezIJdjJNFJRmTgAdaSsqKGq22EmoPA7P");
                });

                cfg.UseEncryptedSerializer(new AesCryptoStreamProvider(new Dostawca("19338219338219338219338219338219"), "1933821933821933"));
            });

            await bus.StartAsync();

            try
            {
                while (true)
                {
                    var key = Console.ReadKey(true).KeyChar;
                    if (key == 's')
                    {
                        var sendEndpoint = await bus.GetSendEndpoint(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd/kontroler"));
                        await sendEndpoint.Send<IUstaw>(new { Dziala = true });
                        Console.WriteLine("Wysłano polecenie: start");
                    }
                    else if (key == 't')
                    {
                        var sendEndpoint = await bus.GetSendEndpoint(new Uri("rabbitmq://hawk.rmq.cloudamqp.com/ugawydjd/kontroler"));
                        await sendEndpoint.Send<IUstaw>(new { Dziala = false }, ctx =>
                        {
                            ctx.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader, Guid.NewGuid().ToString());
                        });
                        Console.WriteLine("Wysłano polecenie: stop");
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
