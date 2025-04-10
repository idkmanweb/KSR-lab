using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcf3_2.ServiceReference1;

namespace wcf3_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Zadanie1Client client = new Zadanie1Client();

            string wynik = client.ScalNapisy("Hello ", "world");

            Console.WriteLine("Wynik: " + wynik);

            client.Close();

            Console.ReadLine();
        }
    }
}
