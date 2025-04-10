using System;
using System.ServiceModel;

namespace wcf3_6
{

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Wybierz tryb:");
            Console.WriteLine("1 - Serwis1");
            Console.WriteLine("2 - Serwis2");
            Console.WriteLine("3 - Router");
            Console.WriteLine("4 - Klient");

            switch (Console.ReadLine())
            {
                case "1": HostService(new Service1(), "http://localhost:8001/Service1"); break;
                case "2": HostService(new Service2(), "http://localhost:8002/Service2"); break;
                case "3": HostService(new Router(), "http://localhost:8000/Router"); break;
                case "4": RunClient(); break;
                default: Console.WriteLine("1-4"); break;
            }
        }

        static void HostService(object service, string address)
        {
            var baseAddress = new Uri(address);
            var host = new ServiceHost(service.GetType(), baseAddress);
            host.AddServiceEndpoint(typeof(IService), new BasicHttpBinding(), "");
            host.Open();
            Console.WriteLine($"{service.GetType().Name} uruchomiony pod {address}");
            Console.ReadLine();
            host.Close();
        }

        static void RunClient()
        {
            var factory = new ChannelFactory<IService>(
                new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/Router"));
            var proxy = factory.CreateChannel();

            Console.Write("A = ");
            int a = int.Parse(Console.ReadLine());
            Console.Write("B = ");
            int b = int.Parse(Console.ReadLine());

            int wynik = proxy.Dodaj(a, b);
            Console.WriteLine($"Wynik: {wynik}");
            Console.ReadKey();
        }
    }
}
