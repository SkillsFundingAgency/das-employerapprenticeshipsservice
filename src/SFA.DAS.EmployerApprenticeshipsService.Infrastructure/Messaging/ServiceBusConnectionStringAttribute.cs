using System;

namespace SFA.DAS.EAS.Infrastructure.Messaging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceBusConnectionStringAttribute : Attribute
    {
        public string Name { get; }

        public ServiceBusConnectionStringAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Service bus connection string name cannot be null or white space.", nameof(name));
            }

            Name = name;
        }
    }
}