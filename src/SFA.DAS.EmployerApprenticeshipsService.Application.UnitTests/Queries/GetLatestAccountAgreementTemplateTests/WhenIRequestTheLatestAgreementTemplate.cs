using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLatestAccountAgreementTemplate;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLatestAccountAgreementTemplateTests
{
    public class WhenIRequestTheLatestAgreementTemplate
    {
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private GetLatestAccountAgreementTemplateQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _handler = new GetLatestAccountAgreementTemplateQueryHandler(_employerAgreementRepository.Object);
        }

        [Test]
        public async Task ThenIShouldGetTheLatestTemplateFromTheRepository()
        {
            var template = new EmployerAgreementTemplate();
            _employerAgreementRepository.Setup(x => x.GetLatestAgreementTemplate()).ReturnsAsync(template);

            var response = await _handler.Handle(new GetLatestAccountAgreementTemplateRequest());

            Assert.AreEqual(template, response.Template);
        }
    }
}
