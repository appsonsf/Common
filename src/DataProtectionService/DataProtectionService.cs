using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppsOnSF.Common.BaseServices;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace DataProtectionService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class DataProtectionService : StatefulService, IDataProtectionService
    {

        public DataProtectionService(StatefulServiceContext context) : base(context)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task<List<XElement>> GetAllDataProtectionElements()
        {
            ServiceEventSource.Current.ServiceMessage(Context, "Start GetAllDataProtectionElements");

            var elements = new List<XElement>();

            var dictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, XElement>>("AspNetCore.DataProtection");
            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerable = await dictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                var token = new CancellationToken();

                while (await enumerator.MoveNextAsync(token))
                {
                    elements.Add(enumerator.Current.Value);
                }
            }

            var ids = string.Empty;
            if (ServiceEventSource.Current.IsEnabled())
                ids = string.Join(",", elements.Select(o => o.Attribute("id").Value));
            ServiceEventSource.Current.ServiceMessage(Context,
                "End GetAllDataProtectionElements, elements count: {0}, elements ids: {1}", elements.Count, ids);

            return elements;
        }

        public async Task<XElement> AddDataProtectionElement(XElement element)
        {
            Guid id = Guid.Parse(element.Attribute("id").Value);

            ServiceEventSource.Current.ServiceMessage(Context,
                "Invoke AddDataProtectionElement, elemens id: {0}", id);

            var dictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, XElement>>("AspNetCore.DataProtection");
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await dictionary.GetOrAddAsync(tx, id, element);
                await tx.CommitAsync();

                return result;
            }
        }

    }
}
