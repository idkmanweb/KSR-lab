using KSR_WCF2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace wcf2_1
{
    [ServiceContract]
    public interface IZadanie1
    {
        [OperationContract]
        string DlugieObliczenia();

        [OperationContract]
        string Szybciej(int x, int fx);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDlugieObliczenia(AsyncCallback callback, object state);

        string EndDlugieObliczenia(IAsyncResult result);
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new ServiceReference1.Zadanie1Client();

            IAsyncResult res = client.BeginDlugieObliczenia(null, null);

            for (int x = 0; x<=20; x++)
            {
                client.Szybciej(x, 3*x*x-2*x);
            }

            string wynik = client.EndDlugieObliczenia(res);

            Console.WriteLine("Wynik: " + wynik);

        }
    }
}
