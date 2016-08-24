using System;
using System.Linq;
using Moq;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.Messaging;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.MockPolicy
{
    public class MockMessagePolicy : ConfiguredInstancePolicy
    {
        private readonly Mock<IMessagePublisher> _messagePublisher;

        public MockMessagePolicy(Mock<IMessagePublisher> messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var messagePublisher = instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher) || x.ParameterType == typeof(IPollingMessageReceiver));


            if (messagePublisher != null)
            {
                var queueName = instance
                    .SettableProperties().FirstOrDefault(c => c.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == nameof(QueueNameAttribute)) != null);


                if (queueName != null)
                {
                    instance.Dependencies.AddForConstructorParameter(messagePublisher, _messagePublisher.Object);
                }
            }
        }
    }
}
