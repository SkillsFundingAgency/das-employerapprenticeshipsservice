using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Types;
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
        public AccountDocument AccountDocument { get; set; }
        public const long AccountId = 123;
        
        public AccountTestsFixture ConstructAccountWithReservedFunding(DateTime? createdDate = null)
        {
            AccountDocument = AccountDocument.Create(AccountId);
            AccountDocument.Account = new Account
            {
                Id = AccountId,
                Name = "LegalEntityName"
            };
            Organisation organisation = new Organisation
            {
                Id = 2,
                Name = "LegalEntityName"
            };
            AccountDocument.Account.Organisations.Add(organisation);
            organisation.Reservations.Add(new Reservation {
                Id = new Guid("EEEEEFAA-9D54-422A-8F46-4FF6B9CC07B6"),
                CourseCode = "CourseId",
                CourseName = "CourseName",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow
            });

            return this;
        }

        public AccountTestsFixture AssertConstructFromMessage()
        {
            AccountDocument.Id.Should().NotBeEmpty();
            AccountDocument.AccountId.Should().Be(AccountId);

            return this;
        }
    }
}
