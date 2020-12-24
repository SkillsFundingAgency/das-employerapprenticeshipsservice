using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;

namespace SFA.DAS.EmployerFinance.UnitTests.Infrastructure.OuterApiRequests
{
    public class WhenBuildingTheGetProviderByIdRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(int id)
        {
            //Arrange
            var actual = new GetProviderRequest(id);
            
            //Act
            actual.GetUrl.Should().Be($"providers/{id}");
        }
    }
}