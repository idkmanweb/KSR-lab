using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Hosting;

namespace wcf3_3
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private const string IndexFilePath = @"C:\Users\potek\source\repos\wcf3-3\wcf3-3\index.xhtml";
        private const string ScriptsFilePath = @"C:\Users\potek\source\repos\wcf3-3\wcf3-3\scripts.js";

        public Stream GetIndexHtml()
        {
            return File.OpenRead(IndexFilePath); 
        }

        public Stream GetScriptsJs()
        {
            return File.OpenRead(ScriptsFilePath); 
        }
    }

}
