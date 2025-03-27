using KSR_WCF1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using wcf1_4.KSR_WCF1;

namespace wcf1_4
{
    namespace KSR_WCF1
    {
        public class Zadanie2 : IZadanie2
        {
            public string Test(string arg)
            {
                return $"Otrzymano: {arg}";
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Zadanie2));
            host.AddServiceEndpoint(typeof(IZadanie2),
            new NetNamedPipeBinding(),
            "net.pipe://localhost/ksr-wcf1-zad2");

            NetTcpBinding tcpBinding = new NetTcpBinding();
            host.AddServiceEndpoint(typeof(IZadanie2), tcpBinding, "net.tcp://127.0.0.1:55765/");

            var b = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (b == null)
            {
                b = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(b);
            }

            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
            MetadataExchangeBindings.CreateMexNamedPipeBinding(),
            "net.pipe://localhost/ksr-wcf1-zad2/metadane");

            host.Open();
            Console.ReadKey();
            host.Close();

        }
    }
}
