using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    /// <summary>
    ///     A property bag that allows an <see cref="IIdProvider"/> and <see cref="IProcessor"/> to exchange parameters.
    /// </summary>
    public class ProcessingContext
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public T Get<T>(string name)
        {
            if (TryGet(name, out T result))
            {
                return result;
            }

            throw new ProcessorException($"Required context value '{name}' does not exist.");
        }

        public bool TryGet<T>(string name, out T value)
        {
            if (!_values.TryGetValue(name, out var resultAsString))
            {
                value = default(T);
                return false;
            }

            value = (T) Convert.ChangeType(resultAsString, typeof(T));

            return true;
        }

        public void Set<T>(string name, T value)
        {
            _values[name] = value?.ToString();
        }
    }
}