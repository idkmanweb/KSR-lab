using KSR_WCF2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace wcf2_5
{
    public class Zadanie5Service : IZadanie5
    {
        public string ScalNapisy(string a, string b)
        {
            return a + b;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:15163/");

            ServiceHost selfHost = new ServiceHost(typeof(Zadanie5Service), baseAddress);

            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();

                selfHost.AddServiceEndpoint(typeof(IZadanie5), binding, baseAddress);

                selfHost.Open();
                Console.ReadLine();
                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine(ce.Message);
                selfHost.Abort();
            }
        }
    }
}
