using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace wcf3_1
{
    [ServiceContract]
    public interface IZadanie1
    {
        [OperationContract]
        string ScalNapisy(string a, string b);
    }

    public class Zadanie1 : IZadanie1
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
            string baseAddress = "http://localhost:30703/";

            using (ServiceHost host = new ServiceHost(typeof(Zadanie1), new Uri(baseAddress)))
            {
                host.AddServiceEndpoint(
                    typeof(IZadanie1),
                    new BasicHttpBinding(),
                    ""
                );

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = new Uri(baseAddress)
                };
                host.Description.Behaviors.Add(smb);

                host.Open();
                Console.ReadLine();
                host.Close();
            }
        }
    }
}
