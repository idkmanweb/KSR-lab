using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using KSR_WCF2;

namespace wcf2_3
{
    public class Zadanie3Callback : IZadanie3Zwrotny
    {
        public void WolanieZwrotne(int x, int fx)
        {
            Console.WriteLine($"callback: x = {x}, f(x) = {fx}");
        }
    }

    public class Zadanie3 : IZadanie3
    {
        private IZadanie3Zwrotny callback;

        public Zadanie3()
        {
            callback = OperationContext.Current.GetCallbackChannel<IZadanie3Zwrotny>();
        }

        public void TestujZwrotny()
        {
            for (int x = 0; x <= 30; x++)
            {
                callback.WolanieZwrotne(x, x*x*x - x*x);
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Zadanie3));

            host.AddServiceEndpoint(typeof(IZadanie3),
            new NetNamedPipeBinding(),
            "net.pipe://localhost/ksr-wcf2-zad3");

            host.Open();

            var instanceContext = new InstanceContext(new Zadanie3Callback());
            var fact = new DuplexChannelFactory<IZadanie3>(instanceContext, new NetNamedPipeBinding(),
                new EndpointAddress("net.pipe://localhost/ksr-wcf2-zad3"));
            var client = fact.CreateChannel();

            try
            {
                client.TestujZwrotny();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();

            ((IDisposable)client).Dispose();
            fact.Close();
            host.Close();
        }
    }
}
