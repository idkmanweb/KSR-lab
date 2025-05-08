using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komunikaty
{
    public interface IPubl
    {
        int Numer { get; }
    }

    public class Publ : IPubl
    {
        public int Numer { get; set; }
    }

    public interface IUstaw
    {
        bool Dziala { get; }
    }

    public interface IOdpA
    {
        string Kto { get; }
    }

    public interface IOdpB
    {
        string Kto { get; }
    }
}
