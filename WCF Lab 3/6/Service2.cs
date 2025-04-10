using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcf3_6
{
    public class Service2 : IService
    {
        public int Dodaj(int a, int b)
        {
            Console.WriteLine("Service 2:");
            return a + b;
        }
    }
}
