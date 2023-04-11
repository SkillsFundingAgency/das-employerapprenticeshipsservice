using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Queries.GetVacancies;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetVacancies
{
    public class GetVacancies : QueryBaseTest<GetVacanciesRequestHandler, GetVacanciesRequest, GetVacanciesResponse>
    {
        public override GetVacanciesRequest Query { get; set; }
        public override GetVacanciesRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetVacanciesRequest>> RequestValidator { get; set; }

        private Mock<IRecruitService> _recruitService;
        private Vacancy _vacancy;
        private Mock<ILogger<GetVacanciesRequestHandler>> _logger;
        private string _hashedAccountId;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _hashedAccountId = "123ABC";

            _vacancy = new Vacancy();
            _logger = new Mock<ILogger<GetVacanciesRequestHandler>>();

            _recruitService = new Mock<IRecruitService>();
            _recruitService
                .Setup(s => s.GetVacancies(_hashedAccountId, int.MaxValue))
                .ReturnsAsync(new List<Vacancy> { _vacancy });
            RequestHandler = new GetVacanciesRequestHandler(RequestValidator.Object, _logger.Object, _recruitService.Object);

            Query = new GetVacanciesRequest
            {
                HashedAccountId = _hashedAccountId
            };
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _recruitService.Verify(x => x.GetVacancies(_hashedAccountId, int.MaxValue), Times.Once);
        }

     
        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.Contains(_vacancy, (ICollection) response.Vacancies);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            return Task.CompletedTask;
        }
    }
}
