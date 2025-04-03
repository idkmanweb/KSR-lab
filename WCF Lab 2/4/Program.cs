using KSR_WCF2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace wcf2_4
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Zadanie4 : IZadanie4
    {
        private int licznik;

        public void Ustaw(int v)
        {
            licznik = v;
        }

        public int Dodaj(int v)
        {
            licznik += v;
            return licznik;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Zadanie4));

            host.AddServiceEndpoint(typeof(IZadanie4),
            new NetNamedPipeBinding(),
            "net.pipe://localhost/ksr-wcf2-zad4");

            host.Open();

            var fact = new ChannelFactory<IZadanie4>(new NetNamedPipeBinding(),
                new EndpointAddress("net.pipe://localhost/ksr-wcf2-zad4"));
            var client = fact.CreateChannel();

            Console.ReadKey();

            ((IDisposable)client).Dispose();
            fact.Close();
            host.Close();
        }
    }
}
