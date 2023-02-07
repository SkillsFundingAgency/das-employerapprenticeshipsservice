using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers;

[TestFixture]
public class EmployerAgreementControllerTests : FluentTest<EmployerAgreementControllerTestFixtures>
{
    [Test]
    public Task WhenRequestingConfirmRemoveOrganisationPage_AndUserIsUnauthorised_ThenAccessDeniedIsReturned()
    {
        return TestAsync(
            arrange: fixtures => fixtures.Orchestrator.Setup(x => x.GetConfirmRemoveOrganisationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>
                {
                    Exception = new UnauthorizedAccessException(),
                    Status = HttpStatusCode.Unauthorized
                }),
            act: fixtures => fixtures.ConfirmRemoveOrganisation(),
            assert: (fixtures, result) =>
            {
                ViewResult viewResult = result as ViewResult;
                Assert.AreEqual(ControllerConstants.AccessDeniedViewName, viewResult.ViewName);
            });
    }

    [Test]
    public Task WhenRequestingConfirmRemoveOrganisationPage_AndUserIsAuthorised_AndOrganisationCanBeRemoved_ThenConfirmRemoveViewIsReturned()
    {
        return TestAsync(
            arrange: fixtures => fixtures.Orchestrator.Setup(x => x.GetConfirmRemoveOrganisationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>
                {
                    Data = new ConfirmOrganisationToRemoveViewModel()
                    {
                        CanBeRemoved = true
                    }
                }),
            act: fixtures => fixtures.ConfirmRemoveOrganisation(),
            assert: (fixtures, result) =>
            {
                ViewResult viewResult = result as ViewResult;
                Assert.AreEqual(ControllerConstants.ConfirmRemoveOrganisationViewName, viewResult.ViewName);
            }) ;
    }

    [Test]
    public Task WhenRequestingConfirmRemoveOrganisationPage_AndUserIsAuthorised_AndOrganisationCannotBeRemoved_ThenCannotRemoveOrganisationViewIsReturned()
    {
        return TestAsync(
            arrange: fixtures => fixtures.Orchestrator.Setup(x => x.GetConfirmRemoveOrganisationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>
                {
                    Data = new ConfirmOrganisationToRemoveViewModel()
                    {
                        CanBeRemoved = false
                    }
                }),
            act: fixtures => fixtures.ConfirmRemoveOrganisation(),
            assert: (fixtures, result) =>
            {
                ViewResult viewResult = result as ViewResult;
                Assert.AreEqual(ControllerConstants.CannotRemoveOrganisationViewName, viewResult.ViewName);
            });
    }

    [Test]
    public Task ConfirmRemoveOrganisation_WhenIRemoveAlegalEntityFromAnAccount_ThenTheOrchestratorIsCalledToGetTheConfirmRemoveModel()
    {
        return TestAsync(
            fixtures => fixtures.Orchestrator.Setup(x => x.GetConfirmRemoveOrganisationViewModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>
                {
                    Data = new ConfirmOrganisationToRemoveViewModel()
                }),
            act: fixtures => fixtures.ConfirmRemoveOrganisation(),
            assert: (fixtures, result) => fixtures.Orchestrator.Verify(
                x => x.GetConfirmRemoveOrganisationViewModel(fixtures.HashedAccountLegalEntityId, fixtures.HashedAccountId, fixtures.UserId), Times.Once));
    }

    [Test]
    public Task ViewUnsignedAgreements_WhenIViewUnsignedAgreements_ThenIShouldGoStraightToTheUnsignedAgreementIfThereIsOnlyOne()
    {
        return TestAsync(
            arrange: fixtures =>
            {
                fixtures.Mediator.Setup(x => x.Send(It.Is<GetNextUnsignedEmployerAgreementRequest>(y => y.HashedAccountId == fixtures.HashedAccountId), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new GetNextUnsignedEmployerAgreementResponse
                    {
                        HashedAgreementId = fixtures.HashedAgreementId
                    });
            },
            act: fixtures => fixtures.ViewUnsignedAgreements(),
            assert: (fixtures, result) =>
            {
                fixtures.OwinWrapper.Verify(x => x.GetClaimValue(ControllerConstants.UserRefClaimKeyName));
                Assert.IsNotNull(result);
                Assert.AreEqual(result.RouteValues["action"], "AboutYourAgreement");
                Assert.AreEqual(result.RouteValues["agreementId"], fixtures.HashedAgreementId);
            });
    }
        
    [Test]
    public Task ViewAgreementToSign_ShouldReturnAgreements()
    {
        return TestAsync(arrange: fixtures => fixtures.WithUnsignedEmployerAgreement().WithPreviouslySignedAgreement(),
            act: fixtures => fixtures.SignedAgreement(),
            assert: (fixtures, result) =>
                Assert.AreEqual(fixtures.GetSignAgreementViewModel, fixtures.ViewResult.Model));
    }

    [Test]
    public Task AboutYourAgreement_WhenIViewAboutYourAgreementAsLevy_ThenShouldShowTheAboutYourAgreementView()
    {
        return TestAsync(
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
                                AgreementType = AgreementType.Levy
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
        return TestAsync(
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
                                AgreementType = AgreementType.NonLevyExpressionOfInterest
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

    [Test]
    public Task ViewAgreementToSign_WhenIHaveNotSelectedAnOption_ThenAnErrorIsDisplayed()
    {
        return TestAsync(
            fixtures => fixtures.WithUnsignedEmployerAgreement().WithPreviouslySignedAgreement(),
            fixtures => fixtures.Sign(null),
            (fixtures, result) =>
            {
                var viewResult = result.Item1 as ViewResult;
                var model = viewResult.Model as SignEmployerAgreementViewModel;
                var modelState = result.Item2 as ModelStateDictionary;

                Assert.AreEqual(viewResult.ViewName, ControllerConstants.SignAgreementViewName);
                Assert.AreEqual(fixtures.GetSignAgreementViewModel, model);
                Assert.IsTrue(modelState[nameof(model.Choice)].Errors.Count == 1);
            });
    }

    [Test]
    public Task WhenDoYouWantToView_WhenILandOnThePage_ThenTheLegalEntityNameIsCorrect()
    {
        return TestAsync(
            arrange: fixtures =>
            {
                fixtures.Orchestrator
                    .Setup(x => x.GetById(fixtures.HashedAgreementId, fixtures.HashedAccountId, fixtures.UserId))
                    .ReturnsAsync(new OrchestratorResponse<EmployerAgreementViewModel>
                    {
                        Data = new EmployerAgreementViewModel { EmployerAgreement = new EmployerAgreementView { LegalEntityName = fixtures.LegalEntityName } }
                    });
            },
            act: fixtures => fixtures.WhenDoYouWantToView(),
            assert: (fixtures, actualResult) =>
            {
                Assert.IsNotNull(actualResult);
                var viewResult = actualResult as ViewResult;
                Assert.IsNotNull(viewResult);
                var actualModel = viewResult.Model as WhenDoYouWantToViewViewModel;
                Assert.IsNotNull(actualModel);
                Assert.That(actualModel.EmployerAgreement.LegalEntityName, Is.EqualTo(fixtures.LegalEntityName));
            });
    }

    [Test]
    public Task WhenDoYouWantToView_WhenISelectNow_ThenTheAgreementIsShown()
    {
        return TestAsync(
            act: fixtures => fixtures.WhenDoYouWantToView(1, new WhenDoYouWantToViewViewModel { EmployerAgreement = new EmployerAgreementView() }),
            assert: (fixtures, actualResult) =>
            {
                Assert.IsNotNull(actualResult);
                Assert.That(actualResult.RouteValues["action"], Is.EqualTo("SignAgreement"));
            });
    }

    [Test]
    public Task WhenDoYouWantToView_WhenISelectLater_ThenTheHomepageIsShown()
    {
        return TestAsync(
            act: fixtures => fixtures.WhenDoYouWantToView(2, new WhenDoYouWantToViewViewModel{ EmployerAgreement = new EmployerAgreementView() }),
            assert: (fixtures, actualResult) =>
            {
                Assert.IsNotNull(actualResult);
                Assert.That(actualResult.RouteValues["action"], Is.EqualTo("Index"));
                Assert.That(actualResult.RouteValues["controller"], Is.EqualTo("EmployerTeam"));
            });
    }
}

public class EmployerAgreementControllerTestFixtures : FluentTest<EmployerAgreementControllerTestFixtures>
{
    public Mock<EmployerAgreementOrchestrator> Orchestrator;
    public Mock<IAuthenticationService> OwinWrapper;
    public Mock<IMultiVariantTestingService> UserViewTestingService;
    public Mock<ICookieStorageService<FlashMessageViewModel>> FlashMessage;
    public Mock<IMediator> Mediator;
    public Mock<IMapper> Mapper;

    public EmployerAgreementControllerTestFixtures()
    {
        Orchestrator = new Mock<EmployerAgreementOrchestrator>();
        OwinWrapper = new Mock<IAuthenticationService>();
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
            EmployerAgreement = new EmployerAgreementView()
        };

        GetSignAgreementViewModel = new SignEmployerAgreementViewModel();
    }

    public static class Constants
    {
        public const string HashedAccountId = "ABC123";
        public const string UserId = "AFV456TGF";
        public const string HashedAgreementId = "789UHY";
        public const long AccountLegalEntityId = 1234;
        public const string HashedAccountLegalEntityId = "THGHFH";
        public const string LegalEntityName = "FIFTEEN LIMITED";
    }

    public string HashedAccountId => Constants.HashedAccountId;
    public string UserId => Constants.UserId;
    public string HashedAgreementId => Constants.HashedAgreementId;
    public long AccountLegalEntityId => Constants.AccountLegalEntityId;
    public string LegalEntityName => Constants.LegalEntityName;
    public string HashedAccountLegalEntityId => Constants.HashedAccountLegalEntityId;

    public GetEmployerAgreementRequest GetAgreementRequest { get; }

    public EmployerAgreementViewModel GetAgreementToSignViewModel { get; }

    public SignEmployerAgreementViewModel GetSignAgreementViewModel { get; }

    public ViewResult ViewResult { get; set; }

    public EmployerAgreementControllerTestFixtures WithUnsignedEmployerAgreement()
    {
        var agreementResponse = new GetEmployerAgreementResponse
        {
            EmployerAgreement = new AgreementDto { LegalEntity = new AccountSpecificLegalEntityDto { AccountLegalEntityId = AccountLegalEntityId } }
        };

        Mediator.Setup(x => x.Send(It.Is<GetEmployerAgreementRequest>(r => r.AgreementId == GetAgreementRequest.AgreementId && r.HashedAccountId == GetAgreementRequest.HashedAccountId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(agreementResponse);
        var entitiesCountResponse = new GetAccountLegalEntitiesCountByHashedAccountIdResponse
        {
            LegalEntitiesCount = 1
        };

        Mediator.Setup(x => x.Send(It.IsAny<GetAccountLegalEntitiesCountByHashedAccountIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entitiesCountResponse);

        Mapper.Setup(x => x.Map<GetEmployerAgreementResponse, EmployerAgreementViewModel>(agreementResponse))
            .Returns(GetAgreementToSignViewModel);

        Mapper.Setup(x => x.Map<GetEmployerAgreementResponse, SignEmployerAgreementViewModel>(agreementResponse))
            .Returns(GetSignAgreementViewModel);

        Orchestrator.Setup(x => x.GetById(GetAgreementRequest.AgreementId, GetAgreementRequest.HashedAccountId, GetAgreementRequest.ExternalUserId))
            .ReturnsAsync(new OrchestratorResponse<EmployerAgreementViewModel> { Data = GetAgreementToSignViewModel });
      
        return this;
    }

    public EmployerAgreementControllerTestFixtures WithPreviouslySignedAgreement()
    {
        var response = new GetLastSignedAgreementResponse { LastSignedAgreement = new AgreementDto() };

        Mediator.Setup(x =>
                x.Send(It.Is<GetLastSignedAgreementRequest>(r => r.AccountLegalEntityId == AccountLegalEntityId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
            
        return this;
    }

    public EmployerAgreementController CreateController()
    {
        var httpRequestMock = new Mock<HttpRequest>();
        var httpContextMock = new Mock<HttpContext>();
        var controllerContext = new Mock<ControllerContext>();

        var queryCollection = new QueryCollection()
        {
            Keys = {  ControllerConstants.AccountHashedIdRouteKeyName, HashedAccountId  }
        };
        
        httpRequestMock.Setup(x => x.Query).Returns(queryCollection);
        httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);
        controllerContext.Setup(x => x.HttpContext).Returns(httpContextMock.Object);
            
        var controller = new EmployerAgreementController(
            Orchestrator.Object,
            FlashMessage.Object,
            Mediator.Object,
            Mapper.Object,
            Mock.Of<IUrlActionHelper>(),
            Mock.Of<IHttpContextAccessor>());
            
        controller.ControllerContext = controllerContext.Object;
            
        return controller;
    }

    public Task<IActionResult> ConfirmRemoveOrganisation()
    {
        var controller = CreateController();
        return controller.ConfirmRemoveOrganisation(HashedAccountLegalEntityId, HashedAccountId);
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

    public async Task<Tuple<ViewResult, ModelStateDictionary>> Sign(int? choice)
    {
        var controller = CreateController();
        var result = await controller.Sign(HashedAgreementId, HashedAccountId, choice) as ViewResult;
        return new Tuple<ViewResult, ModelStateDictionary>(result, controller.ModelState);
    }

    public async Task<ViewResult> AboutYourAgreement()
    {
        var controller = CreateController();
        ViewResult = await controller.AboutYourAgreement(HashedAgreementId, HashedAccountId) as ViewResult;
        return ViewResult;
    }

    public async Task<ViewResult> WhenDoYouWantToView()
    {
        var controller = CreateController();
        ViewResult = await controller.WhenDoYouWantToView(HashedAgreementId, HashedAccountId) as ViewResult;
        return ViewResult;
    }

    public async Task<RedirectToRouteResult> WhenDoYouWantToView(int? choice, WhenDoYouWantToViewViewModel model)
    {
        var controller = CreateController();
        return await controller.WhenDoYouWantToView(choice, model.EmployerAgreement.HashedAgreementId, model.EmployerAgreement.HashedAccountId) as RedirectToRouteResult;
    }
}