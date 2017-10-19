using System;
using System.Linq;
using System.Reflection;
using Moq;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.TestCommon.MockPolicy
{
    public class MockMessageSubscriberPolicy : ConfiguredInstancePolicy
    {
        private readonly Mock<IMessageSubscriberFactory> _subscriberFactory;

        public MockMessageSubscriberPolicy(Mock<IMessageSubscriberFactory> subscriberFactory)
        {
            _subscriberFactory = subscriberFactory;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var subscriberFactory = GetMessageSubscriberFactoryParameter(instance);

            if (subscriberFactory == null) return;

            instance.Dependencies.AddForConstructorParameter(subscriberFactory, _subscriberFactory.Object);
        }

        private static ParameterInfo GetMessageSubscriberFactoryParameter(IConfiguredInstance instance)
        {
            var factory = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessageSubscriberFactory));

            return factory;
        }
    }
}