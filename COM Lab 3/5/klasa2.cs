using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

[assembly: AssemblyKeyFile("keyfile.snk")]
namespace com3_5
{
    [Guid("F59DA79E-29BB-476C-BFF4-2E9C0ADFDD4D"),
    ComVisible(true),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface Iklasa2
    {
        uint Test(string napis);
    }

    [Guid("F08FB011-E87D-472E-9886-659C2559FB10"),
    ComVisible(true),
    ClassInterface(ClassInterfaceType.None),
    ProgId("KSR20.COM3Klasa.2")]
    public class klasa2 : Iklasa2
    {
        public klasa2() { }
        public uint Test(string napis) {
            Console.WriteLine("Klasa2 dziala poprawnie.\n");
            return 0;
        }
    }
}
