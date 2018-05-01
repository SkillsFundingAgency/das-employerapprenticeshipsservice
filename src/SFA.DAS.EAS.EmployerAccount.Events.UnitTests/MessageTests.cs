using System;
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
        private const string CreatedBy = "TestUser";
        public readonly Guid CreatorUserExternalId = Guid.NewGuid();

        [Test]
        public void WhenAMessageIsSerialisedThenItCanBeDeserialized()
        {
            var testClass = new TestAccountMessageBase(AccountId, TestStringProperty, TestLongProperty, CreatedBy, CreatorUserExternalId);
            var serialized = JsonConvert.SerializeObject(testClass);
            var deserialized = JsonConvert.DeserializeObject<TestAccountMessageBase>(serialized);

            Assert.AreEqual(TestLongProperty, deserialized.MyTestLongProperty);
            Assert.AreEqual(TestStringProperty, deserialized.MyTestStringProperty);
            Assert.AreEqual(AccountId, deserialized.AccountId);
        }

        public class TestAccountMessageBase : AccountMessageBase
        {
            public TestAccountMessageBase(long accountId, string myTestStringProperty, long myTestLongProperty, string creatorName, Guid externalUserId) : base(accountId, creatorName, externalUserId)
            {
                MyTestStringProperty = myTestStringProperty;
                MyTestLongProperty = myTestLongProperty;
            }

            public string MyTestStringProperty { get;  }


            public long MyTestLongProperty { get;  }
        }
    }
}
