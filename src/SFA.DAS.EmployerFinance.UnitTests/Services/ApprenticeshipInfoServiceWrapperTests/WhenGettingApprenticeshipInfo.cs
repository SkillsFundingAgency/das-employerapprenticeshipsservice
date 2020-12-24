using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Caches;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.ApprenticeshipInfoServiceWrapperTests
{
    public class WhenGettingApprenticeshipInfo
    {
        [Test, MoqAutoData]
        public async Task Then_The_Standards_Are_Retrieved_From_The_Api_And_Added_To_Cached(
            GetStandardsResponse apiResponse,
            [Frozen]Mock<IApiClient> apiClient,
            [Frozen]Mock<IInProcessCache> cache,
            ApprenticeshipInfoServiceWrapper service)
        {
            //Arrange
            StandardsView returnData = null;
            apiClient.Setup(x => x.Get<GetStandardsResponse>(It.IsAny<GetStandardsRequest>()))
                .ReturnsAsync(apiResponse);
            cache.Setup(x => x.Exists("Standards")).Returns(false);
            cache.Setup(x => x.Set("Standards", It.IsAny<StandardsView>())).Callback<string,object>(
                (key, value) =>
                {
                    returnData = value as StandardsView;
                });
            
            //Act
            await service.GetStandardsAsync();
            
            //Assert
            apiClient.Verify(x=>x.Get<GetStandardsResponse>(It.IsAny<GetStandardsRequest>()),Times.Once);
            returnData.Standards.Count.Should().Be(apiResponse.Standards.Count);
            returnData.Standards.ToList().First().ShouldBeEquivalentTo(apiResponse.Standards.First(), options=> options
                .Excluding(c=>c.Title)
                .Excluding(c=>c.Code)
                .Excluding(c=>c.CourseName)
            );
        }

        [Test, MoqAutoData]
        public async Task Then_Standards_Retrieved_From_Cache_If_Cached(
            StandardsView cacheData,
            [Frozen]Mock<IApiClient> apiClient,
            [Frozen]Mock<IInProcessCache> cache,
            ApprenticeshipInfoServiceWrapper service)
        {
            //Arrange
            cache.Setup(x => x.Exists("Standards")).Returns(true);
            cache.Setup(x => x.Get<StandardsView>("Standards")).Returns(cacheData);
            
            //Act
            var actual = await service.GetStandardsAsync();
            
            //Assert
            apiClient.Verify(x=>x.Get<GetStandardsResponse>(It.IsAny<GetStandardsRequest>()),Times.Never);
            actual.ShouldBeEquivalentTo(cacheData);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Frameworks_Are_Retrieved_From_The_Api_And_Added_To_Cached(
            GetFrameworksResponse apiResponse,
            [Frozen]Mock<IApiClient> apiClient,
            [Frozen]Mock<IInProcessCache> cache,
            ApprenticeshipInfoServiceWrapper service)
        {
            //Arrange
            FrameworksView returnData = null;
            apiClient.Setup(x => x.Get<GetFrameworksResponse>(It.IsAny<GetFrameworksRequest>()))
                .ReturnsAsync(apiResponse);
            cache.Setup(x => x.Exists("Frameworks")).Returns(false);
            cache.Setup(x => x.Set("Frameworks", It.IsAny<FrameworksView>())).Callback<string,object>(
                (key, value) =>
                {
                    returnData = value as FrameworksView;
                });

            //Act
            await service.GetFrameworksAsync();
            
            //Assert
            apiClient.Verify(x=>x.Get<GetFrameworksResponse>(It.IsAny<GetFrameworksRequest>()),Times.Once);
            returnData.Frameworks.Count.Should().Be(apiResponse.Frameworks.Count);
            returnData.Frameworks.ToList().First().ShouldBeEquivalentTo(apiResponse.Frameworks.First(), options=> options
                .Excluding(c=>c.Title)
                .Excluding(c=>c.ProgrammeType)
            );
        }
        [Test, MoqAutoData]
        public async Task Then_Frameworks_Retrieved_From_Cache_If_Cached(
            FrameworksView cacheData,
            [Frozen]Mock<IApiClient> apiClient,
            [Frozen]Mock<IInProcessCache> cache,
            ApprenticeshipInfoServiceWrapper service)
        {
            //Arrange
            cache.Setup(x => x.Exists("Frameworks")).Returns(true);
            cache.Setup(x => x.Get<FrameworksView>("Frameworks")).Returns(cacheData);
            
            //Act
            var actual = await service.GetFrameworksAsync();
            
            //Assert
            apiClient.Verify(x=>x.Get<GetFrameworksResponse>(It.IsAny<GetFrameworksRequest>()),Times.Never);
            actual.ShouldBeEquivalentTo(cacheData);
        }

    }
}