using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace wcf3_6
{

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        int Dodaj(int a, int b);
    }
}
