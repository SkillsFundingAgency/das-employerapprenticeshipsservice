using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    public class ObjectContext
    {
        private readonly Dictionary<string, object> _entities = new Dictionary<string, object>();
        private readonly Dictionary<int, object> _entitiesId = new Dictionary<int, object>();

        /// <summary>
        /// The endpoint handling the finance worker messages
        /// </summary>
        public IEndpointInstance FinanceJobsServiceBusEndpoint { get; set; }

        ///// <summary>
        ///// The endpoint to publish finance worker messages
        ///// </summary>
        public IEndpointInstance InitiateJobServiceBusEndpoint { get; set; }

        public Dictionary<string, T> GetAll<T>()
        {
            return _entities.Where(m => m.Value.GetType() == typeof(T)).ToDictionary(m => m.Key, m => (T)m.Value);
        }

        public void Set<T>(string key, T value)
        {
            _entities.Add(key, value);
        }


        public T Get<T>(int key)
        {
            try
            {
                return (T)_entitiesId[key];
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public void Set<T>(int key, T value)
        {
            _entitiesId.Add(key, value);
        }
    }
}