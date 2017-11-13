using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.EAS.EmployerAccount.Events.UnitTests
{
    public class MessageTests
    {
        private const string SignedByName = "signedByName";
        private const string TestStringProperty = "TestStringProperty";
        private const long AccountId = 12345;
        private const long TestLongProperty = 54321;

        [Test]
        public void WhenAMessageIsSerializedThenItCanBeDesialized()
        {
            var testClass = new TestMessage(SignedByName, AccountId, TestStringProperty, TestLongProperty);
            var serialized = JsonConvert.SerializeObject(testClass);
            var deserialized = JsonConvert.DeserializeObject<TestMessage>(serialized);

            Assert.AreEqual(TestLongProperty, deserialized.MyTestLongProperty);
            Assert.AreEqual(TestStringProperty, deserialized.MyTestStringProperty);
            Assert.AreEqual(AccountId, deserialized.AccountId);
            Assert.AreEqual(SignedByName, deserialized.SignedByName);
        }

        public class TestMessage : Message
        {
            public TestMessage(string signedByName, long accountId, string myTestStringProperty, long myTestLongProperty) : base(signedByName, accountId)
            {
                MyTestStringProperty = myTestStringProperty;
                MyTestLongProperty = myTestLongProperty;
            }

            public string MyTestStringProperty { get;  }


            public long MyTestLongProperty { get;  }
        }
    }
}
