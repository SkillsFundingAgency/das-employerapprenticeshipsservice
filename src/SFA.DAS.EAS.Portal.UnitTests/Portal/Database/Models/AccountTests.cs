using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Database.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Database.Models
{
    [TestFixture]
    [Parallelizable]
    public class AccountTests : FluentTest<AccountTestsFixture>
    {
        [Test]
        public void ReservedFundingConstructor_WhenConstructWithReservedFunding_ThenShouldBeConstructedWithCoreProperties()
        {
            Test(f => f.ConstructAccountWithReservedFunding(), (f, r) => f.AssertConstructFromMessage());
        }
    }

    public class AccountTestsFixture
    {
        public Account Account { get; set; }
        public const long AccountId = 123;
        public const string MessageId = "DEADBEEF-1111-1111-1111-111111111111";
        public DateTime CreatedDate = new DateTime(2020, 2, 2);
        
        public AccountTestsFixture ConstructAccountWithReservedFunding(DateTime? createdDate = null)
        {
            Account = new Account(AccountId, 2, "LegalEntityName", new Guid("EEEEEFAA-9D54-422A-8F46-4FF6B9CC07B6"),
                "CourseId", "CourseName", DateTime.UtcNow, DateTime.UtcNow, createdDate ?? CreatedDate, MessageId);

            return this;
        }

        public AccountTestsFixture AssertConstructFromMessage()
        {
            Account.Id.Should().NotBeEmpty();
            Account.OutboxData.Should().Equal(new[] {new OutboxMessage(MessageId, CreatedDate)},
                (o1, o2) => o1.MessageId == o2.MessageId && o1.Created == o2.Created);
            Account.AccountId.Should().Be(AccountId);

            return this;
        }
    }
}