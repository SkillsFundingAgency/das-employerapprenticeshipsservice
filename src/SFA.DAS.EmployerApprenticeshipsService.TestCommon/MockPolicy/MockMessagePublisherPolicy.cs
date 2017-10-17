using System;
using System.Linq;
using Moq;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.TestCommon.MockPolicy
{
    public class MockMessagePublisherPolicy : ConfiguredInstancePolicy
    {
        private readonly Mock<IMessagePublisher> _messagePublisher;

        public MockMessagePublisherPolicy(Mock<IMessagePublisher> messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var messagePublisher = instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher));
            
            if (messagePublisher != null)
            { 
                instance.Dependencies.AddForConstructorParameter(messagePublisher, _messagePublisher.Object);
            }
        }
    }
}
