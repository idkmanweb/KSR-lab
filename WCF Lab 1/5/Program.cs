using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSR_WCF1;
using System.ServiceModel;


namespace wcf1_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fact = new ChannelFactory<IZadanie1>(new NetNamedPipeBinding(),
            new EndpointAddress("net.pipe://localhost/ksr-wcf1-test"));
            var client = fact.CreateChannel();
            try { 
                client.RzucWyjatek(true);
            }
            catch (FaultException<KSR_WCF1.Wyjatek> ex)
            {
                Console.WriteLine("wyjatek: " + ex.Detail.opis);
                client.OtoMagia(ex.Detail.magia);
            }
            ((IDisposable)client).Dispose();
            fact.Close();
        }
    }
}