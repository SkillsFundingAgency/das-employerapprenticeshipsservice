using System;
using Newtonsoft.Json;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerAccounts.Factories;

public class GenericEventFactory : IGenericEventFactory
{
    public GenericEvent Create<T>(T value)
    {
        var typeName = typeof(T).Name;

        var serialisedObject = JsonConvert.SerializeObject(value);

        var payload = serialisedObject;

        return new GenericEvent
        {
            Type = typeName,
            Payload = payload,
            CreatedOn = DateTime.Now
        };
    }
}