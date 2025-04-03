using KSR_WCF2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace wcf2_6
{
    [ServiceContract(CallbackContract = typeof(IZadanie6Zwrotny))]
    public interface IZadanie6
    {
        [OperationContract(IsOneWay = true)]
        void Dodaj(int a, int b);
    }

    public interface IZadanie6Zwrotny
    {
        [OperationContract(IsOneWay = true)]
        void Wynik(int wyn);
    }

    public class Zadanie6 : IZadanie6
    {
        public void Dodaj(int a, int b)
        {
            int wynik = a + b;

            var callback = OperationContext.Current.GetCallbackChannel<IZadanie6Zwrotny>();

            callback.Wynik(wynik);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:15164/");

            ServiceHost selfHost = new ServiceHost(typeof(Zadanie6), baseAddress);

            try
            {
                WSDualHttpBinding binding = new WSDualHttpBinding();

                selfHost.AddServiceEndpoint(typeof(IZadanie6), binding, baseAddress);

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
