using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Steps
{
    public class ObjectContext
    {
        private readonly Dictionary<string, object> _entities = new Dictionary<string, object>();

        /// <summary>
        /// The endpoint handling the finance worker messages
        /// </summary>
        public IEndpointInstance FinanceJobsServiceBusEndpoint { get; set; }

        ///// <summary>
        ///// The endpoint to publish finance worker messages
        ///// </summary>
        public IEndpointInstance InitiateJobServiceBusEndpoint { get; set; }

        public T Get<T>(string key)
        {
            try
            {
                return (T)_entities[key];
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public IDictionary<string, T> GetAll<T>()
        {
            return _entities.OfType<T>() as IDictionary<string, T>;
        }

        public void Set<T>(string key, T value)
        {
            _entities.Add(key, value);
        }
    }
}