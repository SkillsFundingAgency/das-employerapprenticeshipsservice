using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.ReferenceData.Api.Client;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ReferenceDataService
{
    public class WhenIGetACharity
    {
        private readonly Mock<IReferenceDataApiClient> _apiClient = new Mock<IReferenceDataApiClient>();
        private Infrastructure.Services.ReferenceDataService _referenceDataService;
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [SetUp]
        public void Arrange()
        {
            _apiClient.Setup(x => x.GetCharity(It.Is<int>(y=> y == 12345))).ReturnsAsync(new ReferenceData.Api.Client.Dto.Charity
            {
                RegistrationNumber = 12345,
                Name = "Test Charity DTO"
            });

            _apiClient.Setup(x => x.GetCharity(It.Is<int>(y => y == 999))).ReturnsAsync(null);

            _mapper.Setup(x => x.Map<ReferenceData.Api.Client.Dto.Charity, Charity>(It.Is<ReferenceData.Api.Client.Dto.Charity>(c=> c!=null && c.RegistrationNumber == 12345)))
                .Returns(new Charity { RegistrationNumber = 12345, Name = "Test Charity"});

            _mapper.Setup(x => x.Map<ReferenceData.Api.Client.Dto.Charity, Charity>(It.Is<ReferenceData.Api.Client.Dto.Charity>(c => c == null)))
                .Returns(() => null);

            _referenceDataService = new Infrastructure.Services.ReferenceDataService(_apiClient.Object, _mapper.Object);
        }

        [Test]
        public async Task ThenTheApiClientIsCalledAndItsResultIsMapped()
        {
            //Act
            var charity = await _referenceDataService.GetCharity(12345);

            //Assert
            _apiClient.Verify(x => x.GetCharity(It.Is<int>(i => i == 12345)), Times.Once());
            _mapper.Verify(x=> x.Map<ReferenceData.Api.Client.Dto.Charity, Charity> (It.Is<ReferenceData.Api.Client.Dto.Charity>(c => c.RegistrationNumber == 12345)), Times.Once);
            Assert.AreEqual("Test Charity", charity.Name);
        }

        [Test]
        public async Task ThenTheServiceReturnsNullIfTheCharityDoesNotExist()
        {
            //Act
            var charity = await _referenceDataService.GetCharity(999);

            //Assert
            _apiClient.Verify(x => x.GetCharity(It.Is<int>(i => i == 999)), Times.Once());
            Assert.IsNull(charity);
        }
    }
}
