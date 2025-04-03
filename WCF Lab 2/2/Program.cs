using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace wcf2_2
{
    public class Zwrotny:ServiceReference1.IZadanie2Callback
    {
        public void Zadanie(string nazwa, int punkty, bool zaliczone)
        {
            Console.WriteLine($"Nazwa: {nazwa}");
            Console.WriteLine($"Punky: {punkty}");
            Console.WriteLine($"Zaliczone: {(zaliczone ? "Tak" : "Nie")}");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var handler = new InstanceContext(new Zwrotny());
            var client = new ServiceReference1.Zadanie2Client(handler);

            client.PodajZadania();
            Console.WriteLine("Czekam na wynik");
            Console.ReadLine();
        }
    }
}
