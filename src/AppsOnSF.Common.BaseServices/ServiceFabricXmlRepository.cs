using System;
using System.Collections.Generic;
using System.Text;

namespace AppsOnSF.Common.BaseServices
{
#if NETSTANDARD2_0
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class ServiceFabricXmlRepository : IXmlRepository
    {
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var proxy = ServiceProxy.Create<IDataProtectionService>(new Uri(Constants.DataProtectionServiceUri));
            var elements = Task.Run(() => proxy.GetAllDataProtectionElements()).GetAwaiter().GetResult();
            return elements.AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var proxy = ServiceProxy.Create<IDataProtectionService>(new Uri(Constants.DataProtectionServiceUri));
            Task.Run(() => proxy.AddDataProtectionElement(element)).GetAwaiter().GetResult();
        }
    }
#endif

}
