using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.EmployerAccount.Events.UnitTests
{
    public class MessageTests
    {
        private const string TestStringProperty = "TestStringProperty";
        private const long AccountId = 12345;
        private const long TestLongProperty = 54321;

        [Test]
        public void WhenAMessageIsSerialisedThenItCanBeDeserialized()
        {
            var testClass = new TestMessage(AccountId, TestStringProperty, TestLongProperty);
            var serialized = JsonConvert.SerializeObject(testClass);
            var deserialized = JsonConvert.DeserializeObject<TestMessage>(serialized);

            Assert.AreEqual(TestLongProperty, deserialized.MyTestLongProperty);
            Assert.AreEqual(TestStringProperty, deserialized.MyTestStringProperty);
            Assert.AreEqual(AccountId, deserialized.AccountId);
        }

        public class TestMessage : Message
        {
            public TestMessage(long accountId, string myTestStringProperty, long myTestLongProperty) : base(accountId)
            {
                MyTestStringProperty = myTestStringProperty;
                MyTestLongProperty = myTestLongProperty;
            }

            public string MyTestStringProperty { get;  }


            public long MyTestLongProperty { get;  }
        }
    }
}
