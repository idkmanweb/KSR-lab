using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace wcf3_4
{
    // UWAGA: możesz użyć polecenia „Zmień nazwę” w menu „Refaktoryzuj”, aby zmienić nazwę klasy „Service1” w kodzie, usłudze i pliku konfiguracji.
    // UWAGA: aby uruchomić klienta testowego WCF w celu przetestowania tej usługi, wybierz plik Service1.svc lub Service1.svc.cs w eksploratorze rozwiązań i rozpocznij debugowanie.
    public class Service1 : IService1
    {
        private const string IndexFilePath = @"C:\Users\potek\source\repos\wcf3-4\wcf3-4\index.xhtml";
        private const string ScriptsFilePath = @"C:\Users\potek\source\repos\wcf3-4\wcf3-4\scripts.js";

        public Stream GetIndexHtml()
        {
            return File.OpenRead(IndexFilePath);
        }

        public Stream GetScriptsJs()
        {
            return File.OpenRead(ScriptsFilePath);
        }
        public int Dodaj(string a, string b)
        {
            try
            {
                return Int32.Parse(a) + Int32.Parse(b);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Parametry muszą być typu int");
            }
        }
    }
}
