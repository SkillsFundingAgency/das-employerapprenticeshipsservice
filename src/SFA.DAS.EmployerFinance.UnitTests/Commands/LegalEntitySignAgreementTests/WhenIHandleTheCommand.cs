using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.LegalEntitySignAgreement;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.LegalEntitySignAgreementTests
{
    public class WhenIHandleTheCommand
    {
        private LegalEntitySignAgreementCommandHandler _handler;
        private Mock<IAccountLegalEntityRepository> _accountLegalEntityRepository;

        [SetUp]
        public void Arrange()
        {
            _accountLegalEntityRepository = new Mock<IAccountLegalEntityRepository>();

            _handler = new LegalEntitySignAgreementCommandHandler(_accountLegalEntityRepository.Object,
                Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenTheAccountLegalEntityIsUpdatedWIthTheSignedAgreement()
        {
            var signedAgreementId = 338921;
            var signedAgreementVersion = 3;
            var accountId = 10862;
            var legalEntityId = 44893;

            await _handler.Handle(new LegalEntitySignAgreementCommand(signedAgreementId, signedAgreementVersion, accountId, legalEntityId));

            _accountLegalEntityRepository.Verify(x => x.SignAgreement(signedAgreementId, signedAgreementVersion, accountId, legalEntityId));
        }
    }
}
