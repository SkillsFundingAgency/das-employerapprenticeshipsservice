using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests.WhenCallToActionToggleIsEnabled
{
    public class WhenSetupSingleApprenticeStatus
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IMultiVariantTestingService> mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> mockEmployerTeamOrchestrator;

        [SetUp]
        public void Arrange()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthorizationService = new Mock<IAuthorizationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();

            mockAuthorizationService.Setup(m => m.IsAuthorized("EmployerFeature.HomePage")).Returns(false);
            mockAuthorizationService.Setup(m => m.IsAuthorized("EmployerFeature.CallToAction")).Returns(true);

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object,
                mockAuthorizationService.Object);
        }

        [Test]
        public void ThenForNonLevyTheYourSingleApprovedApprenticeViewIsReturnedAtRow1Panel1()
        {
            //Arrange
            var model = new AccountDashboardViewModel()
            {
                PayeSchemeCount = 1,
                ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy,
                CallToActionViewModel = new CallToActionViewModel
                {
                    Reservations = new List<Reservation> { new Reservation { Status = ReservationStatus.Completed } },
                    Apprenticeships = new List<ApprenticeshipViewModel>()
                            {
                                new ApprenticeshipViewModel()
                                {
                                    ApprenticeshipStatus =ApprenticeshipStatus.Approved
                                }
                            }
                }
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SingleApprenticeshipApproved", (result.Model as dynamic).ViewName);
        }


        [Test]
        public void ThenForNonLevyTheYourSingleApprenticeWithTrainingProviderStatusViewIsReturnedAtRow1Panel1()
        {
            //Arrange
            var model = new AccountDashboardViewModel()
            {
                PayeSchemeCount = 1,
                ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy,
                CallToActionViewModel = new CallToActionViewModel
                {
                    Reservations = new List<Reservation> { new Reservation { Status = ReservationStatus.Completed } },
               
                    Cohorts = new List<CohortViewModel>
                    {
                        new CohortViewModel
                        {
                            //CohortsCount = 1,
                            NumberOfDraftApprentices = 1,
                            CohortStatus = CohortStatus.WithTrainingProvider,
                            Apprenticeships = new List<ApprenticeshipViewModel>()
                            {
                                new ApprenticeshipViewModel
                                {
                                    ApprenticeshipStatus = ApprenticeshipStatus.Draft,
                                    NumberOfDraftApprentices = 1
                                }
                            }
                        }
                    }                                       
                }
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SingleApprenticeshipWithTrainingProvider", (result.Model as dynamic).ViewName);
        }


        [Test]
        public void ThenForNonLevyTheYourSingleApprenticeReadyForReviewStatusViewIsReturnedAtRow1Panel1()
        {
            //Arrange
            var model = new AccountDashboardViewModel()
            {
                PayeSchemeCount = 1,
                ApprenticeshipEmployerType = Common.Domain.Types.ApprenticeshipEmployerType.NonLevy,                
                CallToActionViewModel = new CallToActionViewModel
                {                    
                    Reservations = new List<Reservation> { new Reservation { Status = ReservationStatus.Completed } },              
                    Cohorts = new List<CohortViewModel>
                    {
                        new CohortViewModel
                        {
                            //CohortsCount = 1,
                            NumberOfDraftApprentices = 1,
                            CohortStatus = CohortStatus.Review,
                            Apprenticeships = new List<ApprenticeshipViewModel>()
                            {
                                new ApprenticeshipViewModel
                                {
                                    ApprenticeshipStatus = ApprenticeshipStatus.Draft,
                                    NumberOfDraftApprentices = 1
                                }
                            }
                        }
                    }                                      
                }
            };

            //Act
            var result = _controller.Row1Panel1(model) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SingleApprenticeshipReadyForReview", (result.Model as dynamic).ViewName);
        }
    }
}
