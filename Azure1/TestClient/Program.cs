using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.ServiceReference1;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new Service1Client();

            Console.WriteLine("Create: " + client.Create("user2", "haslo123"));

            Guid? sessionId = client.Login("user2", "haslo123");
            Console.WriteLine("Login session: " + sessionId);

            if (sessionId != null)
            {
                var putResult = client.Put("test.txt", "To jest treść pliku.", sessionId.Value);
                Console.WriteLine("Put result: " + putResult);
            }

            if (sessionId != null)
            {
                var content = client.Get("test.txt", sessionId.Value);
                Console.WriteLine("Get result: " + content);
            }

            Console.WriteLine("Logout: " + client.Logout("user2"));

            Console.ReadKey();
            client.Close();
        }
    }
}
