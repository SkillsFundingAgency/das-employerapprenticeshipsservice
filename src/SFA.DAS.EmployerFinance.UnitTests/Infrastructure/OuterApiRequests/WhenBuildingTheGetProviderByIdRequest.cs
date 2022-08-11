using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Providers;

namespace SFA.DAS.EmployerFinance.UnitTests.Infrastructure.OuterApiRequests
{
    public class WhenBuildingTheGetProviderByIdRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            //Arrange
            var id = 123;
            var actual = new GetProviderRequest(id);
            
            //Act
            actual.GetUrl.Should().Be($"providers/{id}");
        }
    }
}