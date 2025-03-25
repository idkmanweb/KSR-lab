using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com3_2
{

    internal class Program
    {

        static void Main(string[] args)
        {
            Type t = Type.GetTypeFromProgID("KSR20.COM3Klasa.1");
            object k = Activator.CreateInstance(t);

            MethodInfo method = t.GetMethod("Test");
            if (method != null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                foreach (var param in parameters)
                {
                    Console.WriteLine($"Parametr: {param.Name}, Typ: {param.ParameterType}");
                }
            }

            t.InvokeMember("Test", System.Reflection.BindingFlags.InvokeMethod, null, k, args);

        }
    }
}

