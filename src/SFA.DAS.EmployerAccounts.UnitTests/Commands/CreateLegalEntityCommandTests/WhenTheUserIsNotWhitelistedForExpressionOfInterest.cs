using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenTheUserIsNotWhitelistedForExpressionOfInterest : CreateLegalEntityCommandTests
    {
        [SetUp]
        public override void Arrange()
        {
            base.Arrange();
        }

        [Test]
        public async Task ThenACombinedAgreementIsCreated()
        {
            var accountId = 12345;
            EncodingService.Setup(x => x.Decode(Command.HashedAccountId, EncodingType.AccountId)).Returns(accountId);
            EmployerAgreementRepository.Setup(x => x.GetAccountAgreements(accountId)).ReturnsAsync(
                new List<EmployerAgreement>
                {
                    new EmployerAgreement { Template = new AgreementTemplate { AgreementType = AgreementType.Levy } }
                });

            await CommandHandler.Handle(Command, CancellationToken.None);

            AccountRepository.Verify(x =>
                x.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(y =>
                    y.AgreementType == AgreementType.Combined)));
        }       
    }
}
