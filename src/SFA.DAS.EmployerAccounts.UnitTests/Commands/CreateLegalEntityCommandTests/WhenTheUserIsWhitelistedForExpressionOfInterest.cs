using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenTheUserIsWhitelistedForExpressionOfInterest : CreateLegalEntityCommandTests
    {
        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            AuthorizationService.Setup(x => x.IsAuthorized(FeatureType.ExpressionOfInterest)).Returns(true);
        }

        [Test]
        public async Task ThenAnExpressionOfInterestAgreementIsCreated()
        {
            await CommandHandler.Handle(Command);

            AccountRepository.Verify(x =>
                x.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(y =>
                    y.AgreementType == AgreementType.NonLevyExpressionOfInterest)));
        }
    }
}
