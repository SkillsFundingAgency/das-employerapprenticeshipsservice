using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Apprenticeship;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture()]
    public class WhenMappingApprenticeshipSearchQuery
    {
        private ApprenticeshipFiltersMapper _mapper;
        private ApprenticeshipFiltersViewModel _filtersViewModel;

        [SetUp]
        public void Arrange()
        {
            _mapper = new ApprenticeshipFiltersMapper();

            _filtersViewModel = new ApprenticeshipFiltersViewModel
            {
                Status = new List<string>
                {
                    ApprenticeshipStatus.Live.ToString(),
                    ApprenticeshipStatus.Paused.ToString()
                },
                RecordStatus = new List<string>
                {
                    RecordStatus.NoActionNeeded.ToString(),
                    RecordStatus.ChangesForReview.ToString(),
                    RecordStatus.ChangeRequested.ToString()
                },
                Provider = new List<string>
                {
                    "12345"
                },
                Course = new List<string>
                {
                    "CourseId1", "CourseId2", "CourseId3", "CourseId4"
                }
            };
        }

        [Test]
        public void ThenApprenticeshipStatusesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(2, result.ApprenticeshipStatuses.Count);
            Assert.AreEqual((int)ApprenticeshipStatus.Live, (int)result.ApprenticeshipStatuses[0]);
        }

        [Test]
        public void ThenRecordStatusesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(3, result.RecordStatuses.Count);
            Assert.AreEqual((int)RecordStatus.NoActionNeeded, (int)result.RecordStatuses[0]);
        }

        [Test]
        public void ThenProvidersAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(1, result.TrainingProviderIds.Count);
            Assert.AreEqual(12345, result.TrainingProviderIds[0]);
        }

        [Test]
        public void ThenCoursesAreMappedCorrectly()
        {
            //Act
            var result = _mapper.MapToApprenticeshipSearchQuery(_filtersViewModel);

            //Assert
            Assert.AreEqual(4, result.TrainingCourses.Count);
            Assert.AreEqual("CourseId1", result.TrainingCourses[0]);
        }
    }
}
