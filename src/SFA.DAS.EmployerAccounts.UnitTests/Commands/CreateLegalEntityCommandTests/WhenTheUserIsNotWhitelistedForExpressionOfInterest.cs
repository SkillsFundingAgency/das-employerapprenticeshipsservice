using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenTheUserIsNotWhitelistedForExpressionOfInterest : CreateLegalEntityCommandTests
    {
        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            AuthorizationService.Setup(x => x.IsAuthorized(FeatureType.ExpressionOfInterest)).Returns(false);
        }

        [Test]
        public async Task ThenAnExpressionOfInterestAgreementIsCreated()
        {
            var accountId = 12345;
            HashingService.Setup(x => x.DecodeValue(Command.HashedAccountId)).Returns(accountId);
            EmployerAgreementRepository.Setup(x => x.GetAccountAgreements(accountId)).ReturnsAsync(
                new List<EmployerAgreement>
                {
                    new EmployerAgreement { Template = new AgreementTemplate { AgreementType = AgreementType.Levy } }
                });

            await CommandHandler.Handle(Command);

            AccountRepository.Verify(x =>
                x.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(y =>
                    y.AgreementType == AgreementType.Levy)));
        }

        [Test]
        public async Task IfAnExpressionOfInterestAgreementAlreadyExistsThenAnExpressionOfInterestAgreementIsCreated()
        {
            EmployerAgreementRepository.Setup(x => x.GetAccountAgreements(AccountId)).ReturnsAsync(
                new List<EmployerAgreement>
                {
                    new EmployerAgreement { Template = new AgreementTemplate { AgreementType = AgreementType.NonLevyExpressionOfInterest } }
                });

            await CommandHandler.Handle(Command);

            AccountRepository.Verify(x =>
                x.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(y =>
                    y.AgreementType == AgreementType.NonLevyExpressionOfInterest)));
        }
    }
}
