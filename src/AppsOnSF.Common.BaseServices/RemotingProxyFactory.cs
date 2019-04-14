using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppsOnSF.Common.BaseServices
{
    public static class RemotingProxyFactory
    {
        public static ISimpleKeyValueService CreateSimpleKeyValueService() =>
            ServiceProxy.Create<ISimpleKeyValueService>(new Uri(Constants.SimpleKeyValueServiceUri));
    }
}
