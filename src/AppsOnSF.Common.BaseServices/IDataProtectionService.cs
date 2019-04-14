using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppsOnSF.Common.BaseServices
{
    public interface IDataProtectionService : IService
    {
        Task<XElement> AddDataProtectionElement(XElement element);
        Task<List<XElement>> GetAllDataProtectionElements();
    }
}
