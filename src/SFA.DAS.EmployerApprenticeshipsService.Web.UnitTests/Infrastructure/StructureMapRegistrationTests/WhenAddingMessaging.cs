using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.DepedencyResolution;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.FileSystem;
using StructureMap;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Infrastructure.StructureMapRegistrationTests
{
    public class WhenAddingMessaging
    {
        private Container _container;

        [SetUp]
        public void Arrange()
        {
            _container = new Container(
                c =>
                    {
                        c.AddRegistry<TestRegistry>();
                        c.Policies.Add(new MessagePolicy("SFA.DAS.EmployerApprenticeshipsService"));
                    }
                );
        }

        [Test]
        public void ThenTheMessageQueueIsTakenFromTheQueueNameAttribute()
        {
            //Act
            var actual = _container.GetInstance<TestClass>();

            //Assert
            Assert.IsAssignableFrom<AzureServiceBusMessageService>(actual.MessagePublisher);
            var actualMessageService = actual.MessagePublisher as AzureServiceBusMessageService;

            Assert.IsNotNull(actualMessageService);
            var queueNameField = actualMessageService
                                    .GetType()
                                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .ToList()
                                    .FirstOrDefault(c => c.Name == "_queueName");
            Assert.IsNotNull(queueNameField);
            Assert.AreEqual(nameof(TestClass.das_queue_name), queueNameField.GetValue(actualMessageService));
            
        }

        [Test]
        public void ThenFileBasedMessageQueueIsResolvedIfNoConnnectionStringForServiceBusIsSupplied()
        {
            //Arrange
            _container = new Container(
                c =>
                    {
                        c.AddRegistry<TestRegistry>();
                        c.Policies.Add(new MessagePolicy("SFA.DAS.EmployerApprenticeshipsService_no_SB"));
                    }
                );

            //Act
            var actual = _container.GetInstance<TestClass>();

            //Assert
            Assert.IsAssignableFrom<FileSystemMessageService>(actual.MessagePublisher);
            var actualMessageService = actual.MessagePublisher as FileSystemMessageService;

            Assert.IsNotNull(actualMessageService);
            var storageDirectoryField = actualMessageService
                                    .GetType()
                                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .ToList()
                                    .FirstOrDefault(c => c.Name == "_storageDirectory");
            Assert.IsNotNull(storageDirectoryField);
            var expectedDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TestClass.das_queue_name));
            Assert.AreEqual(expectedDirectory, storageDirectoryField.GetValue(actualMessageService));
        }

        public interface ITestClass
        {

        }

        public class TestClass : ITestClass
        {
            [QueueName]
            public string das_queue_name { get; set; }

            public readonly IMessagePublisher MessagePublisher;

            public TestClass(IMessagePublisher messagePublisher)
            {
                MessagePublisher = messagePublisher;
            }
        }
        public class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<ITestClass>().Use<TestClass>();
            }
        }
    }
}
