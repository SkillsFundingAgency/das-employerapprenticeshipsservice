using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.ManagedCompany;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ManagedCompanyLookupServiceTests
{
    public class WhenIGetManagedCompanies
    {
        private ManagedCompanyLookupService _managedCompanyLookupService;
        private Mock<ICacheProvider> _cacheProvider;
        private Mock<ManagedCompanyLookupService> _mockManagedCompanyLookupService;

        [SetUp]
        public void Arrange()
        {
            _cacheProvider = new Mock<ICacheProvider>();
            _cacheProvider.SetupSequence(c => c.Get<ManagedCompanyLookup>(nameof(ManagedCompanyLookup)))
                .Returns(null)
                .Returns(new ManagedCompanyLookup());

            _mockManagedCompanyLookupService = new Mock<ManagedCompanyLookupService>(_cacheProvider.Object);
            _mockManagedCompanyLookupService.Setup(x => x.GetDataFromStorage()).Returns(new ManagedCompanyLookup());
            _mockManagedCompanyLookupService.Setup(x => x.GetCompanies()).CallBase();
            _managedCompanyLookupService = _mockManagedCompanyLookupService.Object;
        }

        [Test]
        public void ThenTheItemsAreReadFromTheCacheOnSubsequentCalls()
        {
            //Act
            _managedCompanyLookupService.GetCompanies();
            _managedCompanyLookupService.GetCompanies();

            //Assert
            _mockManagedCompanyLookupService.Verify(x => x.GetDataFromStorage(), Times.Once());
            _cacheProvider.Verify(x => x.Get<ManagedCompanyLookup>(nameof(ManagedCompanyLookup)), Times.Exactly(2));
        }

        [Test]
        public void ThenTheValueIsNotAddedToTheCacheIfNullOrEmpty()
        {
            //Arrange
            _mockManagedCompanyLookupService.Setup(x => x.GetDataFromStorage()).Returns((ManagedCompanyLookup)null);

            //Act
            _managedCompanyLookupService.GetCompanies();

            //Assert
            _cacheProvider.Verify(x => x.Set(nameof(FeatureToggleLookup), It.IsAny<FeatureToggleLookup>(), It.IsAny<TimeSpan>()), Times.Never);
        }
    }
}
