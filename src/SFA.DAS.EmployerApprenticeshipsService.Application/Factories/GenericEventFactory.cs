using System;
using Newtonsoft.Json;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Factories
{
    public class GenericEventFactory : IGenericEventFactory
    {
        public GenericEvent Create<T>(T value) where T : IEventView
        {
            var typeName = typeof(T).Name;

            var serialisedObject = JsonConvert.SerializeObject(value);

            var payload = serialisedObject;

            return new GenericEvent
            {
                Event = value.Event,
                Type = typeName,
                Payload = payload,
                CreatedOn = DateTime.Now
            };
        }
    }
}
