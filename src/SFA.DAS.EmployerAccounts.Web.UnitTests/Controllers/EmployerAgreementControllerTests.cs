﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Testing;


namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers
{
    [TestFixture]
    public class EmployerAgreementControllerTests : FluentTest<EmployerAgreementControllerTestFixtures>
    {
        [Test]
        public Task GetOrganisationsToRemove_WhenIRemoveAlegalEntityFromAnAccount_ThenTheOrchestratorIsCalledToGetAccountsToRemove()
        {
            return RunAsync(
                act: fixtures => fixtures.GetOrganisationsToRemove(),
                assert: (fixtures, result) => fixtures.Orchestrator.Verify(x => x.GetLegalAgreementsToRemove(fixtures.HashedAccountId, fixtures.UserId), Times.Once));
        }

        [Test]
        public Task ConfirmRemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheOrchestratorIsCalledToGetTheConfirmRemoveModel()
        {
            return RunAsync(
                act: fixtures => fixtures.ConfirmRemoveOrganisation(),
                assert: (fixtures, result) => fixtures.Orchestrator.Verify(
                    x => x.GetConfirmRemoveOrganisationViewModel(fixtures.HashedAgreementId, fixtures.HashedAccountId, fixtures.UserId), Times.Once));
        }

        [Test]
        public Task ConfirmRemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheFlashMessageIsPopulatedFromTheCookieWhenGettingTheConfirmRemoveAction()
        {
            return RunAsync(
                arrange: fixtures =>
                {
                    fixtures.FlashMessage
                        .Setup(x => x.Get("sfa-das-employerapprenticeshipsservice-flashmessage"))
                        .Returns(new FlashMessageViewModel { Headline = "" });

                    fixtures.Orchestrator
                        .Setup(x => x.GetConfirmRemoveOrganisationViewModel(fixtures.HashedAgreementId, fixtures.HashedAccountId, fixtures.UserId))
                        .ReturnsAsync(new OrchestratorResponse<ConfirmLegalAgreementToRemoveViewModel>
                        {
                            Data = new ConfirmLegalAgreementToRemoveViewModel()
                        });
                },
                act: fixtures => fixtures.ConfirmRemoveOrganisation(),
                assert: (fixtures, actualResult) =>
                {
                    Assert.IsNotNull(actualResult);
                    var viewResult = actualResult as ViewResult;
                    Assert.IsNotNull(viewResult);
                    var actualModel = viewResult.Model as OrchestratorResponse<ConfirmLegalAgreementToRemoveViewModel>;
                    Assert.IsNotNull(actualModel);
                    Assert.IsNotNull(actualModel.FlashMessage);
                });
        }

        //[Test]
        //public Task RemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheOrchestratorIsCalledToRemoveTheOrg()
        //{
        //    return RunAsync(
        //        arrange: fixtures => fixtures.Orchestrator
        //                                .Setup(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), fixtures.UserId))
        //                                .ReturnsAsync(new OrchestratorResponse<bool> { Status = HttpStatusCode.OK, FlashMessage = new FlashMessageViewModel() }),
        //        act: fixtures => fixtures.RemoveOrganisation(),
        //        assert: (fixtures, result) => fixtures.Orchestrator
        //                .Verify(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), fixtures.UserId), Times.Once)
        //        );
        //}

        //[TestCase(HttpStatusCode.Accepted, "Index", 0)]
        //[TestCase(HttpStatusCode.BadRequest, "ConfirmRemoveOrganisation", 1)]
        //[TestCase(HttpStatusCode.OK, "Index", 1)]
        //public Task RemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheActionRedirectsToTheCorrectViewWhenRemovingTheOrg(HttpStatusCode code, string viewName, int isFlashPopulated)
        //{
        //    return RunAsync(
        //        arrange: fixtures => fixtures.Orchestrator
        //            .Setup(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), fixtures.UserId))
        //            .ReturnsAsync(new OrchestratorResponse<bool> { Status = code, FlashMessage = new FlashMessageViewModel() }),
        //        act: fixtures => fixtures.RemoveOrganisation(),
        //        assert: (fixtures, actualResult) =>
        //        {
        //            Assert.IsNotNull(actualResult);
        //            var redirectResult = actualResult as RedirectToRouteResult;
        //            Assert.IsNotNull(redirectResult);
        //            Assert.AreEqual(viewName, redirectResult.RouteValues["Action"]);
        //            fixtures.FlashMessage.Verify(x => x.Create(It.IsAny<FlashMessageViewModel>(), "sfa-das-employerapprenticeshipsservice-flashmessage", 1), Times.Exactly(isFlashPopulated));
        //        });
        //}

        [Test]
        public Task NextSteps_WhenIViewNextSteps_ThenShouldShowIfUserCanSeeWizardWhenViewingNextSteps()
        {
            return RunAsync(
                arrange: fixtures =>
                {
                    fixtures.OwinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(fixtures.UserId);
                    fixtures.Orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(true);
                },
                act: fixtures => fixtures.NextSteps(),
                assert: (fixtures, actualResult) =>
                {
                    Assert.IsNotNull(actualResult);
                    Assert.IsTrue(actualResult.Data.UserShownWizard);
                    fixtures.Orchestrator.Verify(x => x.UserShownWizard(fixtures.UserId, fixtures.HashedAccountId), Times.Once);
                }
            );
        }

        [Test]
        public Task NextSteps_WhenIViewNextSteps_ThenShouldShowIfUserCanSeeWizardWhenSelectingIncorrectChoiceForNextSteps()
        {
            return RunAsync(
                arrange: fixtures =>
                {
                    fixtures.OwinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(fixtures.UserId);
                    fixtures.Orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(true);
                },
                act: fixtures => fixtures.NextSteps(),
                assert: (fixtures, actualResult) =>
                {
                    Assert.IsNotNull(actualResult);
                    Assert.IsTrue(actualResult.Data.UserShownWizard);
                    fixtures.Orchestrator.Verify(x => x.UserShownWizard(fixtures.UserId, fixtures.HashedAccountId), Times.Once);
                });
        }

        [Test]
        public Task ViewUnsignedAgreements_WhenIViewUnsignedAgreements_ThenIShouldGoStraightToTheUnsignedAgreementIfThereIsOnlyOne()
        {
            return RunAsync(
                arrange: fixtures =>
                {
                    fixtures.Orchestrator.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(new OrchestratorResponse<EmployerAgreementListViewModel>
                        {
                            Data = new EmployerAgreementListViewModel
                            {
                                EmployerAgreementsData = new GetAccountEmployerAgreementsResponse
                                {
                                    EmployerAgreements = new List<EmployerAgreementStatusDto>
                                    {
                                        new EmployerAgreementStatusDto
                                        {
                                            Pending = new PendingEmployerAgreementDetailsDto
                                            {
                                                HashedAgreementId = fixtures.HashedAgreementId,
                                                Id = 123
                                            }
                                        },
                                        new EmployerAgreementStatusDto
                                        {
                                            Signed = new SignedEmployerAgreementDetailsDto
                                            {
                                                HashedAgreementId = "JH4545",
                                                Id = 456
                                            }
                                        }
                                    }
                                }
                            }
                        });
                },
                act: fixtures => fixtures.ViewUnsignedAgreements(),
                assert: (fixtures, result) =>
                {
                    fixtures.OwinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
                    fixtures.Orchestrator.Verify(x => x.Get(fixtures.HashedAccountId, fixtures.UserId));
                    Assert.IsNotNull(result);
                    Assert.AreEqual(result.RouteValues["action"], "AboutYourAgreement");
                    Assert.AreEqual(result.RouteValues["agreementId"], fixtures.HashedAgreementId);
                });
        }

        //[Test]
        //public Task ThenIShouldSeeAllAgreementsIfIHaveMoreThanASingleUnsignedAgreement()
        //{
        //    return RunAsync(
        //        arrange: fixtures =>
        //        {
        //            fixtures.Orchestrator.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
        //                .ReturnsAsync(new OrchestratorResponse<EmployerAgreementListViewModel>
        //                {
        //                    Data = new EmployerAgreementListViewModel
        //                    {
        //                        EmployerAgreementsData =
        //                            new GetAccountEmployerAgreementsResponse
        //                            {
        //                                EmployerAgreements = new List<EmployerAgreementStatusDto>
        //                                {
        //                                    new EmployerAgreementStatusDto
        //                                    {
        //                                        Pending = new PendingEmployerAgreementDetailsDto
        //                                        {
        //                                            HashedAgreementId = "GHJ356"
        //                                        }
        //                                    },
        //                                    new EmployerAgreementStatusDto
        //                                    {
        //                                        Pending = new PendingEmployerAgreementDetailsDto
        //                                        {
        //                                            HashedAgreementId = "JH4545"
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                    }
        //                });
        //        },
        //        act: fixtures => fixtures.ViewUnsignedAgreements(),
        //        assert: (fixtures, actualResult) =>
        //        {
        //            fixtures.OwinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
        //            fixtures.Orchestrator.Verify(x => x.Get(fixtures.HashedAccountId, fixtures.UserId));
        //            Assert.IsNotNull(actualResult);
        //            Assert.AreEqual(actualResult.RouteValues["action"], "Index");
        //        }
        //    );
        //}

        [Test]
        public Task ViewAgreementToSign_ShouldReturnAgreements()
        {
            return RunAsync(arrange: fixtures => fixtures.WithUnsignedEmployerAgreement(),
                act: fixtures => fixtures.SignedAgreement(),
                assert: (fixtures, result) =>
                    Assert.AreEqual(fixtures.GetAgreementToSignViewModel, fixtures.ViewResult.Model));
        }

        [Test]
        public Task AboutYourAgreement_WhenIViewAboutYourAgreementAsLevy_ThenShouldShowTheAboutYourAgreementView()
        {
            return RunAsync(
                arrange: fixtures =>
                {
                    fixtures.OwinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(fixtures.UserId);
                    fixtures.Orchestrator.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(new OrchestratorResponse<EmployerAgreementViewModel>
                        {
                            Data = new EmployerAgreementViewModel
                            {
                                EmployerAgreement = new EmployerAgreementView
                                {
                                    TemplateAgreementType = AgreementType.Levy
                                }
                            }
                        });
                },
                act: fixtures => fixtures.AboutYourAgreement(),
                assert: (fixtures, actualResult) =>
                {
                    Assert.IsNotNull(actualResult);                   
                    Assert.AreEqual(actualResult.ViewName, "AboutYourAgreement");
                });
        }

        [Test]
        public Task AboutYourAgreement_WhenIViewAboutYourAgreementAsEoi_ThenShouldShowTheAboutYourAgreementView()
        {
            return RunAsync(
                arrange: fixtures =>
                {
                    fixtures.OwinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(fixtures.UserId);
                    fixtures.Orchestrator.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(new OrchestratorResponse<EmployerAgreementViewModel>
                        {
                            Data = new EmployerAgreementViewModel
                            {
                                EmployerAgreement = new EmployerAgreementView
                                {
                                    TemplateAgreementType = AgreementType.NoneLevyExpressionOfInterest
                                }
                            }
                        });
                },
                act: fixtures => fixtures.AboutYourAgreement(),
                assert: (fixtures, actualResult) =>
                {
                    Assert.IsNotNull(actualResult);
                    Assert.AreEqual(actualResult.ViewName, "AboutYourDocument");
                });
        }
    }

    public class EmployerAgreementControllerTestFixtures : FluentTestFixture
    {
        public Mock<EmployerAgreementOrchestrator> Orchestrator;
        public Mock<IAuthenticationService> OwinWrapper;
        public Mock<IAuthorizationService> FeatureToggle;
        public Mock<IMultiVariantTestingService> UserViewTestingService;
        public Mock<ICookieStorageService<FlashMessageViewModel>> FlashMessage;
        public Mock<IMediator> Mediator;
        public Mock<IMapper> Mapper;

        public EmployerAgreementControllerTestFixtures()
        {
            Orchestrator = new Mock<EmployerAgreementOrchestrator>();
            OwinWrapper = new Mock<IAuthenticationService>();
            FeatureToggle = new Mock<IAuthorizationService>();
            UserViewTestingService = new Mock<IMultiVariantTestingService>();
            FlashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            OwinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(Constants.UserId);
            Mediator = new Mock<IMediator>();
            Mapper = new Mock<IMapper>();

            GetAgreementRequest = new GetEmployerAgreementRequest
            {
                ExternalUserId = UserId,
                HashedAccountId = HashedAccountId,
                AgreementId = HashedAgreementId
            };

            GetAgreementToSignViewModel = new EmployerAgreementViewModel
            {
                EmployerAgreement = new EmployerAgreementView(),
                PreviouslySignedEmployerAgreement = new EmployerAgreementView()
            };
        }

        public static class Constants
        {
            public const string HashedAccountId = "ABC123";
            public const string UserId = "AFV456TGF";
            public const string HashedAgreementId = "789UHY";
        }

        public string HashedAccountId => EmployerAgreementControllerTestFixtures.Constants.HashedAccountId;
        public string UserId => EmployerAgreementControllerTestFixtures.Constants.UserId;
        public string HashedAgreementId => EmployerAgreementControllerTestFixtures.Constants.HashedAgreementId;

        public GetEmployerAgreementRequest GetAgreementRequest { get; }

        public EmployerAgreementViewModel GetAgreementToSignViewModel { get; }

        public ViewResult ViewResult { get; set; }

        public EmployerAgreementControllerTestFixtures WithUnsignedEmployerAgreement()
        {
            var response = new GetEmployerAgreementResponse
            {
                EmployerAgreement = new AgreementDto()
            };

            Mediator.Setup(x => x.SendAsync(GetAgreementRequest))
                    .ReturnsAsync(response);

            Mapper.Setup(x => x.Map<GetEmployerAgreementResponse, EmployerAgreementViewModel>(response))
                .Returns(GetAgreementToSignViewModel);

            return this;
        }

        public EmployerAgreementController CreateController()
        {
            var controller = new EmployerAgreementController(
                OwinWrapper.Object,
                Orchestrator.Object,
                FeatureToggle.Object,
                UserViewTestingService.Object,
                FlashMessage.Object,
                Mediator.Object,
                Mapper.Object);

            return controller;
        }

        public Task<ActionResult> GetOrganisationsToRemove()
        {
            var controller = CreateController();
            return controller.GetOrganisationsToRemove(HashedAccountId);
        }

        public Task<ActionResult> ConfirmRemoveOrganisation()
        {
            var controller = CreateController();
            return controller.ConfirmRemoveOrganisation(HashedAgreementId, HashedAccountId);
        }

        //public Task<ActionResult> RemoveOrganisation()
        //{
        //    var controller = CreateController();
        //    return controller.RemoveOrganisation(HashedAgreementId, HashedAccountId, new ConfirmLegalAgreementToRemoveViewModel
        //    {

        //    });
        //}

        public async Task<OrchestratorResponse<EmployerAgreementNextStepsViewModel>> NextSteps()
        {
            var controller = CreateController();
            var result = await controller.NextSteps(HashedAccountId) as ViewResult;
            var model = result?.Model as OrchestratorResponse<EmployerAgreementNextStepsViewModel>;
            return model;
        }

        public async Task<RedirectToRouteResult> ViewUnsignedAgreements()
        {
            var controller = CreateController();
            var result = await controller.ViewUnsignedAgreements(HashedAccountId) as RedirectToRouteResult;
            return result;
        }

        public async Task<ViewResult> SignedAgreement()
        {
            var controller = CreateController();
            ViewResult = await controller.SignAgreement(GetAgreementRequest) as ViewResult;
            return ViewResult;
        }

        public async Task<ViewResult> AboutYourAgreement()
        {
            var controller = CreateController();
            ViewResult = await controller.AboutYourAgreement(HashedAgreementId, HashedAccountId) as ViewResult;
            return ViewResult;
        }
    }
}
