using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using wcf3_5.ServiceReference1;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Net.Http;
using System.Security.Policy;
using System.Net.Http;
using System.Threading.Tasks;

namespace wcf3_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("a = ");
            var a = int.Parse(Console.ReadLine());
            Console.Write("b = ");
            var b = int.Parse(Console.ReadLine());
            string wynik = Dodaj(a, b).GetAwaiter().GetResult();
            wynik = System.Text.RegularExpressions.Regex.Match(wynik, @"<int[^>]*>(.*?)</int>").Groups[1].Value;
            Console.WriteLine("Wynik: " + wynik);
        }

        static async Task<string> Dodaj(int a, int b)
        {
            string url = $"http://localhost:60313/Service1.svc/Dodaj/{a}/{b}";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(url, null);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
