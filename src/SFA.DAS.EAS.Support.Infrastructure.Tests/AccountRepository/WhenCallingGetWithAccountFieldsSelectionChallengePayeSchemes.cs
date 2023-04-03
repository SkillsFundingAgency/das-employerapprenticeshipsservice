using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetWithAccountFieldsSelectionChallengePayeSchemes : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldReturnTheAccountWithTheChallengedPayeSchemes()
        {
            var id = "123";

            var accountDetailViewModel = new AccountDetailViewModel
            {
                AccountId = 123,
                Balance = 0m,
                PayeSchemes = new ResourceList(
                    new List<ResourceViewModel>
                    {
                        new ResourceViewModel
                        {
                            Id = "123/123456",
                            Href = "https://tempuri.org/payescheme/{1}"
                        }
                    }),
                LegalEntities = new ResourceList(
                    new List<ResourceViewModel>
                    {
                        new ResourceViewModel
                        {
                            Id = "TempUri Limited",
                            Href = "https://tempuri.org/organisation/{1}"
                        }
                    }),
                HashedAccountId = "DFGH",
                DateRegistered = DateTime.Today.AddYears(-2),
                OwnerEmail = "Owner@tempuri.org",
                DasAccountName = "Test Account 1"
            };

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            var obscuredPayePayeScheme = "123/123456";

            PayeSchemeObfuscator.Setup(x => x.ObscurePayeScheme(It.IsAny<string>()))
                .Returns(obscuredPayePayeScheme);


            var payeSchemeViewModel = new PayeSchemeViewModel
            {
                AddedDate = DateTime.Today.AddMonths(-4),
                Ref = "123/123456",
                Name = "123/123456",
                DasAccountId = "123",
                RemovedDate = null
            };

            AccountApiClient.Setup(x => x.GetResource<PayeSchemeViewModel>(It.IsAny<string>()))
                .ReturnsAsync(payeSchemeViewModel);

            var actual = await _sut.Get(id, AccountFieldsSelection.PayeSchemes);

            Logger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));

            PayeSchemeObfuscator.Verify(x => x.ObscurePayeScheme(It.IsAny<string>()), Times.Exactly(2));


            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.PayeSchemes);
            Assert.AreEqual(1, actual.PayeSchemes.Count());

            Assert.IsNull(actual.LegalEntities);
            Assert.IsNull(actual.TeamMembers);
            Assert.IsNull(actual.Transactions);
        }
    }
}