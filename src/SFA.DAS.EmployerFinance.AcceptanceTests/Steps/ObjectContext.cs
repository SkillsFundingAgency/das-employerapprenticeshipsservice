using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    public class ObjectContext
    {
        private readonly Dictionary<string, object> _entities = new Dictionary<string, object>();
        private readonly Dictionary<int, object> _entitiesId = new Dictionary<int, object>();

        public Dictionary<string, T> GetAll<T>()
        {
            return _entities.Where(m => m.Value.GetType() == typeof(T)).ToDictionary(m => m.Key, m => (T)m.Value);
        }

        public void Set<T>(string key, T value)
        {
            _entities.Add(key, value);
        }

        
        public T FirstOrDefault<T>()
        {
            try
            {
                return (T)_entitiesId.FirstOrDefault(e => e.Value.GetType() == typeof(T)).Value;
            }
            catch (Exception)
            {
                return default(T);
            }
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
        
        public T Set<T>(int key, T value)
        {
            _entitiesId.Add(key, value);

            return value;
        }
    }
}