using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komunikaty
{
    public interface IKomunikat
    {
        string text { get; set; }
    }

    public interface IKomunikat2
    {
        string text { get; set; }
    }

    public class Komunikat3 : IKomunikat, IKomunikat2 
    {
        public string text { get;  set; }
    }
}
