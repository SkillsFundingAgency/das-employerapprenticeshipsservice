using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.TrainingCourses;

namespace SFA.DAS.EmployerFinance.UnitTests.Infrastructure.OuterApiRequests
{
    public class WhenBuildingGetFrameworksRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            //Arrange
            var actual = new GetFrameworksRequest();
            
            //Act
            actual.GetUrl.Should().Be("trainingCourses/frameworks");
        }
    }
}