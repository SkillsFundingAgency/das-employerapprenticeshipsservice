using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementTemplateTests
{
    class WhenIRequestTheLatestEmployerAgreementTemplate : 
        QueryBaseTest<
            GetLatestEmployerAgreementTemplateQueryHandler, 
            GetLatestEmployerAgreementTemplateRequest, 
            GetLatestEmployerAgreementResponse>
    {
        public override GetLatestEmployerAgreementTemplateRequest Query { get; set; }
        public override GetLatestEmployerAgreementTemplateQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetLatestEmployerAgreementTemplateRequest>> RequestValidator { get; set; }
     
        private Mock<IEmployerAgreementRepository> _employmentAgreementRepository;
        private EmployerAgreementTemplate _template;

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _template = new EmployerAgreementTemplate
            {
                Id = 10,
                Ref = "324234",
                Text = "template text",
                CreatedDate = DateTime.Now.AddDays(-10),
                ReleasedDate = DateTime.Now.AddDays(-2)
            };

            _employmentAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employmentAgreementRepository.Setup(x => x.GetLatestAgreementTemplate()).ReturnsAsync(_template);

            RequestHandler = new GetLatestEmployerAgreementTemplateQueryHandler(RequestValidator.Object, _employmentAgreementRepository.Object);
            Query = new GetLatestEmployerAgreementTemplateRequest();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employmentAgreementRepository.Verify(x => x.GetLatestAgreementTemplate(), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_template, actual.Template);
        }
    }
}
