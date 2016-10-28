using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateEmployerAgreementTemplate;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateEmployerAgreementTemplateTests
{
    [TestFixture]
    public class WhenICreateAnEmployerAgreementTemplate
    {
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private CreateEmployerAgreementTemplateCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _handler = new CreateEmployerAgreementTemplateCommandHandler(_employerAgreementRepository.Object);
        }

        [Test]
        public void ThenThrowsAnExceptionIfInvalidRequest()
        {
            var response = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(new CreateEmployerAgreementTemplateCommand()));

            Assert.That(response.ErrorMessages.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task ThenCreatesNewTemplate()
        {
            var command = new CreateEmployerAgreementTemplateCommand
            {
                TemplateRef = "T/3",
                Text = "Sample text"
            };

            await _handler.Handle(command);

            _employerAgreementRepository.Verify(x => x.CreateEmployerAgreementTemplate(command.TemplateRef, command.Text), Times.Once);
        }
    }
}