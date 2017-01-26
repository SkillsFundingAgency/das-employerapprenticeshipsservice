using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.ReleaseEmployerAgreementTemplate;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.ReleaseEmployerAgreementTemplateTests
{
    [TestFixture]
    public class WhenIReleaseAnEmployerAgreementTemplate
    {
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private ReleaseEmployerAgreementTemplateCommandHandler _handler;
        private ReleaseEmployerAgreementTemplateCommand _command;

        [SetUp]
        public void Setup()
        {
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _handler = new ReleaseEmployerAgreementTemplateCommandHandler(_employerAgreementRepository.Object);
            _command = new ReleaseEmployerAgreementTemplateCommand
            {
                TemplateId = 77
            };
        }

        [Test]
        public void ThenThrowsAnExceptionIfInvalidRequest()
        {
            var response = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(new ReleaseEmployerAgreementTemplateCommand()));

            Assert.That(response.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenThrowsAnExceptionIfTemplateDoesNotExist()
        {
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreementTemplate(_command.TemplateId))
                .ReturnsAsync(null);

            var response = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(response.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenThrowsAnExceptionIfTemplateAlreadyReleased()
        {
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreementTemplate(_command.TemplateId))
                .ReturnsAsync(new EmployerAgreementTemplate
                {
                    ReleasedDate = DateTimeProvider.Current.UtcNow.AddDays(-1)
                });

            var response = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));

            Assert.That(response.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThenReleasesTheTemplate()
        {
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreementTemplate(_command.TemplateId))
                .ReturnsAsync(new EmployerAgreementTemplate
                {
                    ReleasedDate = null
                });

            await _handler.Handle(_command);

            _employerAgreementRepository.Verify(x => x.ReleaseEmployerAgreementTemplate(_command.TemplateId), Times.Once);
        }
    }
}