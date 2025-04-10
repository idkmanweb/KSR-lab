using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcf3_6
{
    public class Service1 : IService
    {
        public int Dodaj(int a, int b)
        {
            Console.WriteLine("Service 1:");
            return a + b;
        }
    }
}
