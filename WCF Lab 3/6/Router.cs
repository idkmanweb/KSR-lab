using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace wcf3_6
{

    public class Router : IService
    {
        private readonly IService client1;
        private readonly IService client2;

        public Router()
        {
            var b = new BasicHttpBinding();
            client1 = new ChannelFactory<IService>(b, new EndpointAddress("http://localhost:8001/Service1")).CreateChannel();
            client2 = new ChannelFactory<IService>(b, new EndpointAddress("http://localhost:8002/Service2")).CreateChannel();
        }

        public int Dodaj(int a, int b)
        {
            try
            {
                Console.WriteLine("Uzyto Service1");
                return client1.Dodaj(a, b);
            }
            catch
            {
                Console.WriteLine("Uzyto Service2");
                return client2.Dodaj(a, b);
            }
        }
    }

}
