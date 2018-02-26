using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.Domain.Models
{
    public abstract class Entity : IEntity
    {
        private readonly List<Func<object>> _events = new List<Func<object>>();

        protected void Publish<T>(Action<T> action) where T : class, new()
        {
            _events.Add(() =>
            {
                var message = new T();

                action(message);

                return message;
            });
        }

        IEnumerable<object> IEntity.GetEvents()
        {
            return _events.Select(e => e());
        }
    }
}