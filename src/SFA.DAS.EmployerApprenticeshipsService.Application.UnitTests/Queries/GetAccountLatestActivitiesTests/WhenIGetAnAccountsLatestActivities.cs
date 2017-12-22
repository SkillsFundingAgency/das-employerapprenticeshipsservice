using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Queries.GetAccountLatestActivities;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountLatestActivitiesTests
{
    public class WhenIGetAnAccountsLatestActivities : QueryBaseTest<GetAccountLatestActivitiesQueryHandler, GetAccountLatestActivitiesQuery, GetAccountLatestActivitiesResponse>
    {
        private Mock<IActivitiesClient> _activitiesClient;
        public override GetAccountLatestActivitiesQuery Query { get; set; }
        public override GetAccountLatestActivitiesQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLatestActivitiesQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _activitiesClient = new Mock<IActivitiesClient>();

            Query = new GetAccountLatestActivitiesQuery
            {
                AccountId = 2
            };

            RequestHandler = new GetAccountLatestActivitiesQueryHandler(_activitiesClient.Object, RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _activitiesClient.Verify(x => x.GetLatestActivities(Query.AccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var latestActivitiesResult = new AggregatedActivitiesResult();

            _activitiesClient.Setup(x => x.GetLatestActivities(It.IsAny<long>())).ReturnsAsync(latestActivitiesResult);

            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(response?.Result);
            Assert.AreEqual(latestActivitiesResult, response.Result);
        }
    }
}
