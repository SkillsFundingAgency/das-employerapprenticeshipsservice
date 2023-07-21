using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetPensionRegulatorTests
{
    public class WhenISearchPensionRegulator : QueryBaseTest<GetPensionRegulatorQueryHandler, GetPensionRegulatorRequest, GetPensionRegulatorResponse>
    {
        private Mock<IPensionRegulatorService> _pensionRegulatorService;
        public override GetPensionRegulatorRequest Query { get; set; }
        public override GetPensionRegulatorQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPensionRegulatorRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _pensionRegulatorService = new Mock<IPensionRegulatorService>();

            Query = new GetPensionRegulatorRequest() {PayeRef = "123/4567"};

            RequestHandler = new GetPensionRegulatorQueryHandler(RequestValidator.Object, _pensionRegulatorService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            const string payeRef = "123/4567";

            //Act
            await RequestHandler.Handle(new GetPensionRegulatorRequest {PayeRef = payeRef}, CancellationToken.None);

            //Assert
            _pensionRegulatorService.Verify(x => x.GetOrganisationsByPayeRef(payeRef), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var expectedResponse = new List<Organisation>();
            var expectedPayeRef = "123/4567";
            _pensionRegulatorService.Setup(x => x.GetOrganisationsByPayeRef(expectedPayeRef)).ReturnsAsync(expectedResponse);

            //Act
            var actual = await RequestHandler.Handle(new GetPensionRegulatorRequest() { PayeRef = expectedPayeRef }, CancellationToken.None);

            //Assert
            Assert.AreSame(expectedResponse, actual.Organisations);
        }
    }
}
