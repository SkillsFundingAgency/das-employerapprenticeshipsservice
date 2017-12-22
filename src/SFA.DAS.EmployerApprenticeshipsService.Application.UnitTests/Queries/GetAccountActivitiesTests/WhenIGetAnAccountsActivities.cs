using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Queries.GetAccountActivities;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountActivitiesTests
{
    public class WhenIGetAnAccountsActivities : QueryBaseTest<GetAccountActivitiesQueryHandler, GetAccountActivitiesQuery, GetAccountActivitiesResponse>
    {
        private Mock<IActivitiesClient> _activitiesClient;
        public override GetAccountActivitiesQuery Query { get; set; }
        public override GetAccountActivitiesQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountActivitiesQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _activitiesClient = new Mock<IActivitiesClient>();

            Query = new GetAccountActivitiesQuery
            {
                AccountId = 2,
                Take = 100,
                From = DateTime.UtcNow.AddDays(-1),
                To = DateTime.UtcNow,
                Term = "Foo Bar",
                Category = ActivityTypeCategory.Unknown,
                Data = new Dictionary<string, string>
                {
                    ["Foo"] = "Bar"
                }
            };

            RequestHandler = new GetAccountActivitiesQueryHandler(_activitiesClient.Object, RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _activitiesClient.Verify(x => x.GetActivities(It.Is<ActivitiesQuery>(q => 
                q.AccountId == Query.AccountId &&
                q.Take == Query.Take &&
                q.From == Query.From &&
                q.To == Query.To &&
                q.Term == Query.Term &&
                q.Category == Query.Category &&
                q.Data == Query.Data
            )), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var activitiesResult = new ActivitiesResult();

            _activitiesClient.Setup(x => x.GetActivities(It.IsAny<ActivitiesQuery>())).ReturnsAsync(activitiesResult);

            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(response?.Result);
            Assert.AreEqual(activitiesResult, response.Result);
        }
    }
}