//using System;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using NUnit.Framework;
//using SFA.DAS.EAS.Domain.Attributes;
//using SFA.DAS.EAS.Domain.Configuration;
//using SFA.DAS.EAS.Infrastructure.DependencyResolution;
//using SFA.DAS.Messaging;
//using SFA.DAS.Messaging.Attributes;
//using SFA.DAS.Messaging.AzureServiceBus;
//using SFA.DAS.Messaging.FileSystem;
//using SFA.DAS.Messaging.Interfaces;
//using StructureMap;

//TODO Fix these tests - structure map issues


//namespace SFA.DAS.EAS.Web.UnitTests.Infrastructure.StructureMapRegistrationTests
//{
//    public class WhenAddingMessaging
//    {
//        private Container _container;

//        [SetUp]
//        public void Arrange()
//        {
//            _container = new Container(
//                c =>
//                    {
//                        c.AddRegistry<TestRegistry>();
//                        c.Policies.Add(new MessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
//                    }
//                );
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _container.Dispose();
//        }

//        [Test]
//        public void ThenTheMessageQueueIsTakenFromTheQueueNameAttribute()
//        {
//            //Act
//            var actual = _container.GetInstance<TestClass>();

//            //Assert
//            Assert.IsAssignableFrom<TopicMessagePublisher>(actual.MessagePublisher);
//            var actualMessageService = actual.MessagePublisher as TopicMessagePublisher;

//            Assert.IsNotNull(actualMessageService);
//            var queueNameField = actualMessageService
//                                    .GetType()
//                                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
//                                    .ToList()
//                                    .FirstOrDefault(c => c.Name == "_queueName");
//            Assert.IsNotNull(queueNameField);
//            Assert.AreEqual("das_polling_queue_name", queueNameField.GetValue(actualMessageService));

//        }

//        [Test]
//        public void ThenFileBasedMessageQueueIsResolvedIfNoConnnectionStringForServiceBusIsSupplied()
//        {
//            //Arrange
//            _container = new Container(
//                c =>
//                    {
//                        c.AddRegistry<TestRegistry>();
//                        c.Policies.Add(new MessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService_no_SB"));
//                    }
//                );

//            //Act
//            var actual = _container.GetInstance<TestClass>();

//            //Assert
//            Assert.IsAssignableFrom<FileSystemMessagePublisher>(actual.MessagePublisher);
//            var actualMessageService = actual.MessagePublisher as FileSystemMessagePublisher;

//            Assert.IsNotNull(actualMessageService);
//            var storageDirectoryField = actualMessageService
//                                    .GetType()
//                                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
//                                    .ToList()
//                                    .FirstOrDefault(c => c.Name == "_storageDirectory");
//            Assert.IsNotNull(storageDirectoryField);
//            var expectedDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EAS_Queues/", "das_queue_name");
//            Assert.AreEqual(expectedDirectory, storageDirectoryField.GetValue(actualMessageService));
//        }

//        [Test]
//        public void ThenThePollingReceiverIsResolvedThroughThePolicy()
//        {
//            //Arrange
//            _container = new Container(
//                c =>
//                    {
//                        c.AddRegistry<TestRegistryPolling>();
//                        c.Policies.Add(new MessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
//                    }
//                );

//            //Act
//            var actual = _container.GetInstance<TestClassPolling>();

//            Assert.IsAssignableFrom<TopicMessageSubscriber<TestPollingMessage>>(actual.PollingMessageReceiver);
//            var actualMessageService = actual.PollingMessageReceiver as TopicMessageSubscriber<TestPollingMessage>;

//            Assert.IsNotNull(actualMessageService);
//            var queueNameField = actualMessageService
//                                    .GetType()
//                                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
//                                    .ToList()
//                                    .FirstOrDefault(c => c.Name == "_queueName");
//            Assert.IsNotNull(queueNameField);
//            Assert.AreEqual("das_polling_queue_name", queueNameField.GetValue(actualMessageService));
//        }

//        [Test]
//        public void ThenThePollingRecieverWillUseTheConnectionCollectionIfTheAttributeConstructorPropertiesAreSet()
//        {
//            //Arrange
//            _container = new Container(
//                c =>
//                    {
//                        c.AddRegistry<TestRegistryPolling>();
//                        c.Policies.Add(new MessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
//                    }
//                );

//            //Act
//            var actual = _container.GetInstance<TestClass2>();

//            //Assert
//            Assert.IsAssignableFrom<TopicMessagePublisher>(actual.MessagePublisher);
//            var actualMessageService = actual.MessagePublisher as TopicMessagePublisher;

//            Assert.IsNotNull(actualMessageService);
//            var connectionString = actualMessageService
//                                    .GetType()
//                                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
//                                    .ToList()
//                                    .FirstOrDefault(c => c.Name == "_connectionString");
//            Assert.IsNotNull(connectionString);
//            Assert.AreEqual("Test_ServiceBusConnectionString2", connectionString.GetValue(actualMessageService));
//        }

//        [Test]
//        public void ThenThePollingRecieverWillUseTheConstructorAttributeIfPresentToDetermineTheConnection()
//        {
//            //Arrange
//            _container = new Container(
//                c =>
//                {
//                    c.AddRegistry<TestRegistryPolling>();
//                    c.Policies.Add(new MessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
//                }
//                );

//            //Act
//            var actual = _container.GetInstance<TestClass3>();

//            //Assert
//            Assert.IsAssignableFrom<TopicMessagePublisher>(actual.MessagePublisher);
//            var actualMessageService = actual.MessagePublisher as TopicMessagePublisher;

//            Assert.IsNotNull(actualMessageService);
//            var connectionString = actualMessageService
//                                    .GetType()
//                                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
//                                    .ToList()
//                                    .FirstOrDefault(c => c.Name == "_connectionString");
//            Assert.IsNotNull(connectionString);
//            Assert.AreEqual("Test_ServiceBusConnectionString1", connectionString.GetValue(actualMessageService));
//        }

//        public interface ITestClass { }
//        public interface ITestClass2 { }
//        public interface ITestClass3 { }

//        [MessageGroup("das_queue_name")]
//        public class TestClass : ITestClass
//        {
//            public readonly IMessagePublisher MessagePublisher;

//            public TestClass(IMessagePublisher messagePublisher)
//            {
//                MessagePublisher = messagePublisher;
//            }
//        }

//        [MessageGroup("employer_levy")]
//        public class TestClass2 : ITestClass2
//        {
//            public readonly IMessagePublisher MessagePublisher;

//            [ServiceBusConnectionKey("employer_payment")]
//            public TestClass2(IMessagePublisher messagePublisher)
//            {
//                MessagePublisher = messagePublisher;
//            }
//        }

//        public class TestClass3 : ITestClass3
//        {
//            public readonly IMessagePublisher MessagePublisher;

//            [ServiceBusConnectionKey("employer_payment")]
//            public TestClass3(IMessagePublisher messagePublisher)
//            {
//                MessagePublisher = messagePublisher;
//            }
//        }

//        public class TestClassPolling : ITestClass
//        {
//            public readonly IMessageSubscriber<TestPollingMessage> PollingMessageReceiver;

//            public TestClassPolling(IMessageSubscriber<TestPollingMessage> pollingMessageReceiver)
//            {
//                PollingMessageReceiver = pollingMessageReceiver;
//            }
//        }

//        public class TestClassPolling2 : ITestClass2
//        {
//            public readonly IMessageSubscriber<TestPollingMessage> PollingMessageReceiver;

//            public TestClassPolling2(IMessageSubscriber<TestPollingMessage> pollingMessageReceiver)
//            {
//                PollingMessageReceiver = pollingMessageReceiver;
//            }
//        }

//        [MessageGroup("das_polling_queue_name")]
//        public class TestPollingMessage
//        {  }

//        public class TestRegistry : Registry
//        {
//            public TestRegistry()
//            {
//                For<ITestClass>().Use<TestClass>();
//                For<ITestClass2>().Use<TestClass2>();
//                For<ITestClass3>().Use<TestClass3>();
                
//            }
//        }

//        public class TestRegistryPolling : Registry
//        {
//            public TestRegistryPolling()
//            {
//                For<ITestClass>().Use<TestClassPolling>();
//                For<ITestClass2>().Use<TestClassPolling2>();
//            }
//        }
//    }
//}
