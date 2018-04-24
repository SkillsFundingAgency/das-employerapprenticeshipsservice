using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers
{
    [TestFixture]
    public class EmployerAgreementControllerTests : FluentTest<EmployerAgreementControllerTestFixtures>
    {
        [Test]
        public Task GetOrganisationsToRemove_WhenIRemoveAlegalEntityFromAnAccount_ThenTheOrchestratorIsCalledToGetAccountsToRemove()
        {
            return RunAsync(
                act: fixtures => fixtures.GetOrganisationsToRemove(),
                assert: fixtures => fixtures.Orchestrator.Verify(x => x.GetLegalAgreementsToRemove(fixtures.HashedAccountId, fixtures.UserId), Times.Once));
        }

        [Test]
        public Task ConfirmRemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheOrchestratorIsCalledToGetTheConfirmRemoveModel()
        {
            return RunAsync(
                act: fixtures => fixtures.ConfirmRemoveOrganisation(),
                assert: fixtures => fixtures.Orchestrator.Verify(
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

        [Test]
        public Task RemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheOrchestratorIsCalledToRemoveTheOrg()
        {
            return RunAsync(
                arrange: fixtures => fixtures.Orchestrator
                                        .Setup(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), fixtures.UserId))
                                        .ReturnsAsync(new OrchestratorResponse<bool> { Status = HttpStatusCode.OK, FlashMessage = new FlashMessageViewModel() }),
                act: fixtures => fixtures.RemoveOrganisation(),
                assert: fixtures => fixtures.Orchestrator
                        .Verify(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), fixtures.UserId), Times.Once)
                );
        }

        [TestCase(HttpStatusCode.Accepted, "Index", 0)]
        [TestCase(HttpStatusCode.BadRequest, "ConfirmRemoveOrganisation", 1)]
        [TestCase(HttpStatusCode.OK, "Index", 1)]
        public Task RemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheActionRedirectsToTheCorrectViewWhenRemovingTheOrg(HttpStatusCode code, string viewName, int isFlashPopulated)
        {
            return RunAsync(
                arrange: fixtures => fixtures.Orchestrator
                    .Setup(x => x.RemoveLegalAgreement(It.IsAny<ConfirmLegalAgreementToRemoveViewModel>(), fixtures.UserId))
                    .ReturnsAsync(new OrchestratorResponse<bool> { Status = code, FlashMessage = new FlashMessageViewModel() }),
                act: fixtures => fixtures.RemoveOrganisation(),
                assert: (fixtures, actualResult) =>
                {
                    Assert.IsNotNull(actualResult);
                    var redirectResult = actualResult as RedirectToRouteResult;
                    Assert.IsNotNull(redirectResult);
                    Assert.AreEqual(viewName, redirectResult.RouteValues["Action"]);
                    fixtures.FlashMessage.Verify(x => x.Create(It.IsAny<FlashMessageViewModel>(), "sfa-das-employerapprenticeshipsservice-flashmessage", 1), Times.Exactly(isFlashPopulated));
                });
        }

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
                arrange: fixtures => {
                    fixtures.OwinWrapper.Setup(x => x.GetClaimValue(@"sub")).Returns(fixtures.UserId);
                    fixtures.Orchestrator.Setup(x => x.UserShownWizard(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(true);
                },
                act: fixtures => fixtures.NextSteps(),
                assert: (fixtures, actualResult) => {
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
                assert: (fixtures, result) => {
                    fixtures.OwinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));
                    fixtures.Orchestrator.Verify(x => x.Get(fixtures.HashedAccountId, fixtures.UserId));
                    Assert.IsNotNull(result);
                    Assert.AreEqual(result.RouteValues["action"], "AboutYourAgreement");
                    Assert.AreEqual(result.RouteValues["agreementId"], fixtures.HashedAgreementId);
                });
        }

        [Test]
        public Task ThenIShouldSeeAllAgreementsIfIHaveMoreThanASingleUnsignedAgreement()
        {
            return RunAsync(
                arrange: fixtures =>
                {
                    fixtures.Orchestrator.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(new OrchestratorResponse<EmployerAgreementListViewModel>
                        {
                            Data = new EmployerAgreementListViewModel
                            {
                                EmployerAgreementsData =
                                    new GetAccountEmployerAgreementsResponse
                                    {
                                        EmployerAgreements = new List<EmployerAgreementStatusDto>
                                        {
                                            new EmployerAgreementStatusDto
                                            {
                                                Pending = new PendingEmployerAgreementDetailsDto
                                                {
                                                    HashedAgreementId = "GHJ356"
                                                }
                                            },
                                            new EmployerAgreementStatusDto
                                            {
                                                Pending = new PendingEmployerAgreementDetailsDto
                                                {
                                                    HashedAgreementId = "JH4545"
                                                }
                                            }
                                        }
                                    }
                            }
                        });
                },
                act: fixtures => fixtures.ViewUnsignedAgreements(),
                assert: (fixtures, actualResult) =>
                {
                    fixtures.OwinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName));
                    fixtures.Orchestrator.Verify(x => x.Get(fixtures.HashedAccountId, fixtures.UserId));
                    Assert.IsNotNull(actualResult);
                    Assert.AreEqual(actualResult.RouteValues["action"], "Index");
                }
            );
        }
    }

    public class EmployerAgreementControllerTestFixtures
    {
        public Mock<EmployerAgreementOrchestrator> Orchestrator;
        public Mock<IAuthenticationService> OwinWrapper;
        public Mock<IAuthorizationService> FeatureToggle;
        public Mock<IMultiVariantTestingService> UserViewTestingService;
        public Mock<ICookieStorageService<FlashMessageViewModel>> FlashMessage;

        public EmployerAgreementControllerTestFixtures()
        {
            Orchestrator = new Mock<EmployerAgreementOrchestrator>();
            OwinWrapper = new Mock<IAuthenticationService>();
            FeatureToggle = new Mock<IAuthorizationService>();
            UserViewTestingService = new Mock<IMultiVariantTestingService>();
            FlashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            OwinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(Constants.UserId);
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

        public EmployerAgreementController CreateController()
        {
            var controller = new EmployerAgreementController(
                OwinWrapper.Object, 
                Orchestrator.Object, 
                FeatureToggle.Object, 
                UserViewTestingService.Object,
                FlashMessage.Object);

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

        public Task<ActionResult> RemoveOrganisation()
        {
            var controller = CreateController();
            return controller.RemoveOrganisation(HashedAgreementId, HashedAccountId, new ConfirmLegalAgreementToRemoveViewModel
            {
                
            });
        }

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
    }
}
