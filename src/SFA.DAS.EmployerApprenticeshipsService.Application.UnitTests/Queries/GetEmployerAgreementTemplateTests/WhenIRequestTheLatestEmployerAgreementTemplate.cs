using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetEmployerAgreementTemplateTests
{
    class WhenIRequestTheLatestEmployerAgreementTemplate
    {
        private GetLatestEmployerAgreementTemplateQueryHandler _handler;
        private Mock<IEmployerAgreementRepository> _employmentAgreementRepository;
        
        [SetUp]
        public void Arrange()
        {
            _employmentAgreementRepository = new Mock<IEmployerAgreementRepository>();

            _handler = new GetLatestEmployerAgreementTemplateQueryHandler(_employmentAgreementRepository.Object);    
        }

        [Test]
        public async Task ThenIShouldGetTheLatestRepository()
        {
            //Assign
            var template = new EmployerAgreementTemplate
            {
                Id = 10,
                Ref = "324234",
                Text = "template text",
                CreatedDate = DateTime.Now.AddDays(-10),
                ReleasedDate = DateTime.Now.AddDays(-2)
            };

            _employmentAgreementRepository.Setup(x => x.GetLatestAgreementTemplate()).ReturnsAsync(template);

            //Act
            var response = await _handler.Handle(new GetLatestEmployerAgreementTemplateRequest());

            //Assert
            Assert.AreEqual(template, response.Template);

        }
    }
}
