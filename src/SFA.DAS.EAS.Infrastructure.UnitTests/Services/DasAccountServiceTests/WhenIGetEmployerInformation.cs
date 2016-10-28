using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerSchemes;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.DasAccountServiceTests
{
    public class WhenIGetEmployerInformation
    {
        private Mock<IMediator> _mediator;
        private DasAccountService _dasAccountService;
        private const long ExpectedAccountId = 656474;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerSchemesQuery>())).ReturnsAsync(new GetEmployerSchemesResponse {Schemes = new Schemes()});

            _dasAccountService = new DasAccountService(_mediator.Object);
        }

        [Test]
        public async Task ThenTheGetEmployerSchemesQueryIsCalledWithThePassedAccountId()
        {
            //Act
            await _dasAccountService.GetAccountSchemes(ExpectedAccountId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetEmployerSchemesQuery>(c=>c.Id.Equals(ExpectedAccountId))));
        }
    }
}
