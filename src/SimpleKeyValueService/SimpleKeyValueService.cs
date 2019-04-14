using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppsOnSF.Common.BaseServices;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace SimpleKeyValueService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SimpleKeyValueService : StatefulService, ISimpleKeyValueService
    {
        public SimpleKeyValueService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddOrUpdate(string container, string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                await dictionary.AddOrUpdateAsync(tx, key, new DataItem(value),
                    (oldKey, oldValue) => new DataItem(value));
                await tx.CommitAsync();
            }
        }

        public async Task<bool> Add(string container, string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await dictionary.TryAddAsync(tx, key, new DataItem(value));
                await tx.CommitAsync();

                return result;
            }
        }


        public async Task<bool> ContainsKey(string container, string key)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await dictionary.ContainsKeyAsync(tx, key);

                return result;
            }
        }

        public async Task<string> Get(string container, string key)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await dictionary.TryGetValueAsync(tx, key);

                return result.HasValue ? result.Value.Value : null;
            }
        }

        public async Task<string> CheckAndGet(string container, string key, TimeSpan validity)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await dictionary.TryGetValueAsync(tx, key);

                return result.HasValue && DateTimeOffset.UtcNow.Subtract(validity) < result.Value.Timestamp
                    ? result.Value.Value : null;
            }
        }

        public async Task<Dictionary<string, string>> GetAll(string container)
        {
            ServiceEventSource.Current.ServiceRequestStart("GetAll");

            var result = new Dictionary<string, string>();
            try
            {
                var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
                using (var tx = this.StateManager.CreateTransaction())
                {
                    var enumerable = await dictionary.CreateEnumerableAsync(tx);
                    var enumerator = enumerable.GetAsyncEnumerator();
                    var token = new CancellationToken();

                    while (await enumerator.MoveNextAsync(token))
                    {
                        result.Add(enumerator.Current.Key, enumerator.Current.Value.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message("Error: " + ex.Message);
            }

            ServiceEventSource.Current.ServiceRequestStop("GetAll");

            return result;
        }

        public async Task<string> Remove(string container, string key)
        {
            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await dictionary.TryRemoveAsync(tx, key);
                await tx.CommitAsync();

                return result.HasValue ? result.Value.Value : null;
            }
        }

        public async Task<long> Clear(string container, DateTimeOffset before)
        {
            var willDeleting = new List<string>();
            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerable = await dictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                var token = new CancellationToken();

                while (await enumerator.MoveNextAsync(token))
                {
                    if (enumerator.Current.Value.Timestamp < before)
                        willDeleting.Add(enumerator.Current.Key);
                }

                long counter = 0;
                foreach (var key in willDeleting)
                {
                    var result = await dictionary.TryRemoveAsync(tx, key);
                    if (result.HasValue) counter++;
                }
                await tx.CommitAsync();
                return counter;
            }
        }

        public async Task<long> ClearAll(string container)
        {
            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            long counter = 0;
            using (var tx = this.StateManager.CreateTransaction())
            {
                counter = await dictionary.GetCountAsync(tx);
            }
            await StateManager.RemoveAsync(container);
            return counter;
        }

        public async Task<bool> Update(string container, string key, string value)
        {
            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, DataItem>>(container);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var resultOld = await dictionary.TryGetValueAsync(tx, key);
                if (resultOld.HasValue)
                {
                    var result = await dictionary.TryUpdateAsync(tx, key, new DataItem(value), resultOld.Value);
                    await tx.CommitAsync();

                    return result;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }
    }
}
