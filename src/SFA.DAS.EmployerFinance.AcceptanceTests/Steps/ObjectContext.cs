using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    public class ObjectContext
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        
        public T Get<T>()
        {
            return _cache.TryGetValue(typeof(T).FullName, out var value) ? (T)value : default(T);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _cache.Values.OfType<T>();
        }

        public void Set<T>(T value)
        {
            _cache.Add(typeof(T).FullName, value);
        }

        public void Set<T>(string key, T value)
        {
            _cache.Add(key, value);
        }

        public void Set<T>(int key, T value)
        {
            Set(key.ToString(), value);
        }
    }
}