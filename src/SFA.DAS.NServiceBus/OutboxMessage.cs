using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.NServiceBus
{
    public class OutboxMessage
    {
        public virtual string Id { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime? Dispatched { get; protected set; }
        public virtual string Data { get; protected set; }

        public OutboxMessage(IEnumerable<Event> events)
        {
            Id = GuidComb.NewGuidComb().ToString();
            Created = DateTime.UtcNow;
            Data = JsonConvert.SerializeObject(events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        protected OutboxMessage()
        {
        }

        public IEnumerable<Event> Dispatch()
        {
            if (Dispatched != null)
            {
                return new List<Event>();
            }

            Dispatched = DateTime.UtcNow;

            return JsonConvert.DeserializeObject<List<Event>>(Data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}