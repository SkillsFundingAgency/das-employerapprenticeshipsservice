using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using MediatR;

using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenMappingToUpdateModel
    {
        private Mock<IMediator> _mockMediator;

        private ApprenticeshipMapper _mappingTjänst;
        [SetUp]
        public void SetUp()
        {
            var mockHashingService = new Mock<IHashingService>();
            var mockCurrentDateTime = new Mock<ICurrentDateTime>();
            _mockMediator = new Mock<IMediator>();

            _mappingTjänst = new ApprenticeshipMapper(mockHashingService.Object, mockCurrentDateTime.Object, _mockMediator.Object);
        }

        [Test]
        public async Task TwoEmptyModelIsEqueal()
        {
            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(new Apprenticeship(), new ApprenticeshipViewModel());

            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.EmployerRef.Should().BeNull();
        }

        [Test]
        public async Task UpdateEveryField()
        {
            var a = new Apprenticeship
                {
                    FirstName = "Kalle",
                    LastName = "Abba",
                    EmployerRef = "This is a reference",
                    Cost = 4.0M
                };

            var updated = new ApprenticeshipViewModel
                              {
                                  FirstName = "Fredrik",
                                  LastName = "Stockborg",
                                  EmployerRef = "New ref",
                                  Cost = "5"
                              };

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a , updated);

            model.FirstName.Should().Be("Fredrik");
            model.LastName.Should().Be("Stockborg");
            model.EmployerRef.Should().Be("New ref");
            model.Cost.Should().Be("5");
        }

        [TestCase(1.5, "1600")]
        [TestCase(1, "1.5")]
        [TestCase(1600, "1500")]
        [TestCase(1600, "1700")]
        [TestCase(1600, "")]
        public async Task AndUpdatingCostField(double current, string updatedCost)
        {
            var a = new Apprenticeship { Cost = new decimal(current) };

            var updated = new ApprenticeshipViewModel { Cost = updatedCost };

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);
            model.Cost.Should().Be(updatedCost);
        }

        [Test]
        public async Task ShouldUpdateDate()
        {
            var a = new Apprenticeship
            {
                DateOfBirth = new DateTime(1990, 11, 11),
                StartDate = new DateTime(2045, 12, 08),
                EndDate = new DateTime(2046, 12, 08)
            };

            var dob = new DateTimeViewModel(8, 12, 1998);
            var sd = new DateTimeViewModel(08, 5, 2044);
            var ed = new DateTimeViewModel(09, 12, 2047);
            var updated = new ApprenticeshipViewModel
            {
                DateOfBirth = dob,
                StartDate = sd,
                EndDate = ed
            };

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.DateOfBirth.Should().Be(dob);
            model.StartDate.Should().Be(sd);
            model.EndDate.Should().Be(ed);
        }

        [Test]
        public async Task ShouldNotUpdateDatesIfNotChanged()
        {
            var a = new Apprenticeship
            {
                DateOfBirth = new DateTime(1998, 12, 8),
                StartDate = new DateTime(2045, 12, 08),
                EndDate = new DateTime(2046, 12, 08)
            };

            var dob = new DateTimeViewModel(8, 12, 1998);
            var sd = new DateTimeViewModel(08, 12, 2045);
            var ed = new DateTimeViewModel(08, 12, 2046);
            var updated = new ApprenticeshipViewModel
            {
                DateOfBirth = dob,
                StartDate = sd,
                EndDate = ed
            };

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.DateOfBirth.Should().BeNull();
            model.StartDate.Should().BeNull();
            model.EndDate.Should().BeNull();
        }

        [Test]
        public async Task ShouldNotUpdateTrainngCode()
        {
            var a = new Apprenticeship
            {
                TrainingCode = "abba-1234"
            };

            var updated = new ApprenticeshipViewModel
            {
                TrainingCode = "abba-1234"
            };

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.TrainingCode.Should().BeNull();
            model.TrainingName.Should().BeNull();
            model.TrainingType.Should().BeNull();
        }

        [Test]
        public async Task ShouldUpdateTrainngProgramForFramework()
        {
            var a = new Apprenticeship
            {
                TrainingCode = "abba-666"
            };

            var updated = new ApprenticeshipViewModel
            {
                TrainingCode = "abba-555"
            };

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(
                    new GetTrainingProgrammesQueryResponse { TrainingProgrammes = new List<ITrainingProgramme>
                                    {
                        new Framework { Id = "abba-555",  FrameworkCode = 05, PathwayCode = 88, ProgrammeType = 2, Title = "Framework Title"},
                        new Standard { Id = "abba-666" }
                                    }
                    });

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.TrainingCode.Should().Be("abba-555");
            model.TrainingName.Should().Be("Framework Title");
            model.TrainingType.Should().Be(TrainingType.Framework);
        }

        [Test]
        public async Task ShouldUpdateTrainngProgramForStandard()
        {
            var a = new Apprenticeship
            {
                TrainingCode = "abba-666"
            };

            var updated = new ApprenticeshipViewModel
            {
                TrainingCode = "standard-007"
            };

            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(
                    new GetTrainingProgrammesQueryResponse
                    {
                        TrainingProgrammes = new List<ITrainingProgramme>
                                    {
                        new Framework { Id = "abba-555",  FrameworkCode = 05, PathwayCode = 88, ProgrammeType = 2, Title = "Framework Title"},
                        new Standard { Id = "standard-007", Title = "Standard Title", }
                                    }
                    });

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.TrainingCode.Should().Be("standard-007");
            model.TrainingName.Should().Be("Standard Title");
            model.TrainingType.Should().Be(TrainingType.Standard);
        }

        [Test]
        public async Task UpdateReferenceFieldWhenEmpty()
        {
            var a = new Apprenticeship
            {
                FirstName = "Kalle",
                LastName = "Abba",
                EmployerRef = "This is a reference",
                Cost = 4.0M
            };

            var updated = new ApprenticeshipViewModel
            {
                EmployerRef = "",
            };

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.EmployerRef.Should().Be("");
            model.Cost.Should().BeNull();
        }

        [Test]
        public async Task ShouldNotUpdateRefIfNotChanged()
        {
            var a = new Apprenticeship
            {
                FirstName = "Kalle",
                LastName = "Abba",
                EmployerRef = "Hello",
                Cost = 4.0M
            };

            var updated = new ApprenticeshipViewModel
            {
                EmployerRef = null,
            };

            var model = await _mappingTjänst.CompareAndMapToApprenticeshipViewModel(a, updated);

            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.EmployerRef.Should().Be("");
            model.Cost.Should().BeNull();
        }
    }
}
