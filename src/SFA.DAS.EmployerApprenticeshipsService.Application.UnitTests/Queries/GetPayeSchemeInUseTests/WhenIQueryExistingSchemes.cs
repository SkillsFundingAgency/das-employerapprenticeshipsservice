using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetPayeSchemeInUseTests
{
    public class WhenIQueryExistingSchemes : QueryBaseTest<GetPayeSchemeInUseHandler, GetPayeSchemeInUseQuery, GetPayeSchemeInUseResponse>
    {
        private Mock<IEmployerSchemesRepository> _employerSchemesRepository;
        public override GetPayeSchemeInUseQuery Query { get; set; }
        public override GetPayeSchemeInUseHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPayeSchemeInUseQuery>> RequestValidator { get; set; }
        private const long ExpectedAccountId = 12345;

        [SetUp]
        public void Arrange()
        {
            SetUp();
            Query = new GetPayeSchemeInUseQuery {Empref=""};
            _employerSchemesRepository = new Mock<IEmployerSchemesRepository>();
            _employerSchemesRepository.Setup(x => x.GetSchemeByRef(It.IsAny<string>())).ReturnsAsync(new Scheme {AccountId = ExpectedAccountId});

            RequestHandler = new GetPayeSchemeInUseHandler(RequestValidator.Object, _employerSchemesRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            var empref = "123/ABC";

            //Act
            await RequestHandler.Handle(new GetPayeSchemeInUseQuery {Empref = empref});

            //Assert
            _employerSchemesRepository.Verify(x=>x.GetSchemeByRef(empref),Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var empref = "123/ABC";

            //Act
            var actual = await RequestHandler.Handle(new GetPayeSchemeInUseQuery { Empref = empref });

            //Assert
            Assert.IsNotNull(actual.PayeScheme);
            Assert.AreEqual(ExpectedAccountId, actual.PayeScheme.AccountId);
        }
    }
}
