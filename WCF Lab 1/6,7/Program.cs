using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using KSR_WCF1;
using System.ServiceModel.Description;

namespace KSR_WCF1
{
    [ServiceContract]
    public interface IZadanie7
    {
        [OperationContract]
        [FaultContract(typeof(Wyjatek7))]
        void RzucWyjatek7(string a, int b);
    }

    public class Wyjatek7
    {
        public string Opis { get; set; }
        public string A { get; set; }
        public int B { get; set; }
    }

    public class Zadanie7 : IZadanie7
    {
        public void RzucWyjatek7(string a, int b)
        {
            throw new FaultException<Wyjatek7>(new Wyjatek7
            {
                Opis = "Wyjatek w zad7",
                A = a,
                B = b
            });
        }
    }
}

namespace wcf1_67
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Zadanie7));

            host.AddServiceEndpoint(typeof(IZadanie7),
            new NetNamedPipeBinding(),
            "net.pipe://localhost/zad7");

            var b = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (b == null)
            {
                b = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(b);
            }

            host.Open();
            var fact = new ChannelFactory<IZadanie7>(new NetNamedPipeBinding(),
            new EndpointAddress("net.pipe://localhost/zad7"));
            var client = fact.CreateChannel();
            try
            {
                client.RzucWyjatek7("a", 1);
            }
            catch (FaultException<KSR_WCF1.Wyjatek7> ex)
            {
                Console.WriteLine("opis: " + ex.Detail.Opis + ", a: " + ex.Detail.A + ", b: " + ex.Detail.B);
            }
            ((IDisposable)client).Dispose();
            fact.Close();

            Console.ReadKey();
            host.Close();
        }
    }
}
