using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests.WhenHomePageToggleIsEnabled
{
    public class WhenVacancyInProgress
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IMultiVariantTestingService> mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> mockEmployerTeamOrchestrator;
        private Mock<IPortalClient> mockPortalClient;

        [SetUp]
        public void Arrange()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthorizationService = new Mock<IAuthorizationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            mockPortalClient = new Mock<IPortalClient>();

            mockAuthorizationService.Setup(m => m.IsAuthorized("EmployerFeature.CallToAction")).Returns(true);

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object,
                mockPortalClient.Object,
                mockAuthorizationService.Object);
        }

        [Test]
        public void ThenForNonLevyWithASingleDraftVacancyTheVacancyDraftViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,                
                CallToActionViewModel = new CallToActionViewModel
                {
                    AgreementsToSign = false,
                    Reservations = new List<EmployerAccounts.Models.Reservations.Reservation> { new EmployerAccounts.Models.Reservations.Reservation { Status = EmployerAccounts.Models.Reservations.ReservationStatus.Completed } },
                    VacanciesViewModel = new VacanciesViewModel
                    {
                        VacancyCount = 1,
                        Vacancies = new List<VacancyViewModel>{ new VacancyViewModel{ Status = EmployerAccounts.Models.Recruit.VacancyStatus.Draft }
                     }
                    }
                },

                ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("VacancyDraft", (result.Model as dynamic).ViewName);
        }

        [Test]
        public void ThenForNonLevyWithASingleSubmittedVacancyTheVacancyPendingReviewViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                CallToActionViewModel = new CallToActionViewModel
                {
                    AgreementsToSign = false,
                    Reservations = new List<EmployerAccounts.Models.Reservations.Reservation> { new EmployerAccounts.Models.Reservations.Reservation { Status = EmployerAccounts.Models.Reservations.ReservationStatus.Completed } },
                    VacanciesViewModel = new VacanciesViewModel
                    {
                        VacancyCount = 1,
                        Vacancies = new List<VacancyViewModel>{ new VacancyViewModel{ Status = EmployerAccounts.Models.Recruit.VacancyStatus.Submitted }
                     }
                    }
                },

                ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("VacancyPendingReview", (result.Model as dynamic).ViewName);
        }

        [Test]
        public void ThenForNonLevyWithASingleLiveVacancyTheVacancyLiveViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                CallToActionViewModel = new CallToActionViewModel
                {
                    AgreementsToSign = false,
                    Reservations = new List<EmployerAccounts.Models.Reservations.Reservation> { new EmployerAccounts.Models.Reservations.Reservation { Status = EmployerAccounts.Models.Reservations.ReservationStatus.Completed } },
                    VacanciesViewModel = new VacanciesViewModel
                    {
                        VacancyCount = 1,
                        Vacancies = new List<VacancyViewModel>{ new VacancyViewModel{ Status = EmployerAccounts.Models.Recruit.VacancyStatus.Live }
                     }
                    }
                },

                ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("VacancyLive", (result.Model as dynamic).ViewName);
        }

        [Test]
        public void ThenForNonLevyWithASingleClosedVacancyTheVacancyClosedViewIsReturnedAtRow1Panel1()
        {
            // Arrange
            var model = new AccountDashboardViewModel
            {
                PayeSchemeCount = 1,
                CallToActionViewModel = new CallToActionViewModel
                {
                    AgreementsToSign = false,
                    Reservations = new List<EmployerAccounts.Models.Reservations.Reservation> { new EmployerAccounts.Models.Reservations.Reservation { Status = EmployerAccounts.Models.Reservations.ReservationStatus.Completed } },
                    VacanciesViewModel = new VacanciesViewModel
                    {
                        VacancyCount = 1,
                        Vacancies = new List<VacancyViewModel>{ new VacancyViewModel{ Status = EmployerAccounts.Models.Recruit.VacancyStatus.Closed }
                     }
                    }
                },

                ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("VacancyClosed", (result.Model as dynamic).ViewName);
        }
    }
}
