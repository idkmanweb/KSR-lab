using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using KSR_WCF2;

namespace wcf2_7
{
    public class Zadanie6Callback : IZadanie6Zwrotny
    {
        public void Wynik(int wyn)
        {
            Console.WriteLine("callback: " + wyn);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress5 = new Uri("http://localhost:15163/");

            var factory1 = new ChannelFactory<IZadanie5>(
                new BasicHttpBinding(), new EndpointAddress(baseAddress5));

            IZadanie5 proxy1 = factory1.CreateChannel();

            Uri baseAddress6 = new Uri("http://localhost:15164/");
            var callback = new Zadanie6Callback();
            var factory2 = new DuplexChannelFactory<IZadanie6>(callback,
                new WSDualHttpBinding(), new EndpointAddress(baseAddress6));

            IZadanie6 proxy2 = factory2.CreateChannel();

            Console.WriteLine("Test Zadania 5:");
            string result = proxy1.ScalNapisy("Zadanie5", " dziala.");
            Console.WriteLine("\"Zadanie5\" + \" dziala.\" = " + result);

            Console.WriteLine("Test Zadania 6:");
            Console.WriteLine("6 + 6:");
            proxy2.Dodaj(6, 6);

            Console.ReadLine();
            ((IClientChannel)proxy2).Close();
            factory2.Close();
            ((IClientChannel)proxy1).Close();
            factory1.Close();

        }
    }
}
