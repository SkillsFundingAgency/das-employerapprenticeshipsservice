using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetSingleCohort
{
    public class WhenIGetSingleCohort : QueryBaseTest<GetSingleCohortRequestHandler, GetSingleCohortRequest, GetSingleCohortResponse>
    {
        public override GetSingleCohortRequest Query { get; set; }
        public override GetSingleCohortRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetSingleCohortRequest>> RequestValidator { get; set; }
        private Mock<ICommitmentV2Service> _commitmentV2Service;
        private long _accountId;
        private long _cohortId;
        public string HashedAccountId;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountId = 123;
            _cohortId = 1;
            HashedAccountId = "Abc123";
            _commitmentV2Service = new Mock<ICommitmentV2Service>();
            _commitmentV2Service.Setup(m => m.GetCohorts(_accountId))
                .ReturnsAsync(new List<Cohort>() { new Cohort { Id = _cohortId, NumberOfDraftApprentices = 1 }});

            _commitmentV2Service.Setup(m => m.GetDraftApprenticeships(It.IsAny<Cohort>()))
                .ReturnsAsync(new List<Apprenticeship> { new Apprenticeship { CourseName = "CourseName" } });

            RequestHandler = new GetSingleCohortRequestHandler(RequestValidator.Object, _commitmentV2Service.Object,Mock.Of<ILogger<GetSingleCohortRequestHandler>>());

            Query = new GetSingleCohortRequest
            {
                AccountId = _accountId
            };
        }

       
        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert            
            Assert.IsNotNull(response.Cohort);
            Assert.IsTrue(response.Cohort.NumberOfDraftApprentices.Equals(1));
            Assert.IsTrue(response.Cohort?.Apprenticeships.Count().Equals(1));
        }


        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _commitmentV2Service.Verify(x => x.GetCohorts(_accountId), Times.Once);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            return Task.CompletedTask;
        }
    }
}
