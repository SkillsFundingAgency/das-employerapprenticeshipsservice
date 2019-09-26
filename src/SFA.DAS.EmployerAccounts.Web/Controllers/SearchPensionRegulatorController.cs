﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Commands.OrganisationAndPayeRefData;
using SFA.DAS.EmployerAccounts.Commands.OrganisationData;
using SFA.DAS.EmployerAccounts.Commands.PayeRefData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;
using SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts")]
    public class SearchPensionRegulatorController : BaseController
    {
        private readonly SearchPensionRegulatorOrchestrator _searchPensionRegulatorOrchestrator;
        private readonly IMediator _mediatr;
        private const int OrgNotListed = 0;
        private Regex _aornRegex = new Regex("^[A-Z0-9]{13}$");
        private Regex _payeRegex = new Regex("^[0-9]{3}/?[A-Z0-9]{1,7}$");
        private ICookieStorageService<HashedAccountIdModel> _accountCookieStorage;

        public SearchPensionRegulatorController(
            IAuthenticationService owinWrapper,
            SearchPensionRegulatorOrchestrator searchPensionRegulatorOrchestrator,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMediator mediatr,
            ICookieStorageService<HashedAccountIdModel> accountCookieStorage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _searchPensionRegulatorOrchestrator = searchPensionRegulatorOrchestrator;
            _mediatr = mediatr;
            _accountCookieStorage = accountCookieStorage;
        }

        [Route("{HashedAccountId}/pensionregulator", Order = 0)]
        [Route("pensionregulator", Order = 1)]
        public async Task<ActionResult> SearchPensionRegulator(string hashedAccountId)
        {
            var payeRef = _searchPensionRegulatorOrchestrator.GetCookieData().EmployerAccountPayeRefData.PayeReference;

            if (string.IsNullOrEmpty(payeRef))
            {
                return RedirectToAction(ControllerConstants.GatewayViewName, ControllerConstants.EmployerAccountControllerName);
            }

            var model = await _searchPensionRegulatorOrchestrator.SearchPensionRegulator(payeRef);
            model.Data.IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId);

            switch (model.Data.Results.Count)
            {
                case 0:
                    {
                        return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName);
                    }
                case 1:
                    {
                        await SavePensionRegulatorOrganisationDataIfItHasAValidName(model.Data.Results.First(), true, false);
                        return RedirectToAction(ControllerConstants.SummaryActionName, ControllerConstants.EmployerAccountControllerName);
                    }
                default:
                    {
                        return View(ControllerConstants.SearchPensionRegulatorResultsViewName, model.Data);
                    }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/pensionregulator", Order = 0)]
        [Route("pensionregulator", Order = 1)]
        public async Task<ActionResult> SearchPensionRegulator(string hashedAccountId, SearchPensionRegulatorResultsViewModel viewModel)
        {
            if (!viewModel.SelectedOrganisation.HasValue)
            {
                ViewBag.InError = true;
                return View(ControllerConstants.SearchPensionRegulatorResultsViewName, viewModel);
            }

            if (viewModel.SelectedOrganisation == OrgNotListed)
            {
                return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName);
            }

            var item = viewModel.Results.SingleOrDefault(m => m.ReferenceNumber == viewModel.SelectedOrganisation);

            if (item == null) return View(ControllerConstants.SearchPensionRegulatorResultsViewName, viewModel);

            await SavePensionRegulatorOrganisationDataIfItHasAValidName(item, true, true);
            return RedirectToAction(ControllerConstants.SummaryActionName, ControllerConstants.EmployerAccountControllerName);
        }

        [HttpGet]
        [Route("{HashedAccountId}/pensionregulator/aorn", Order = 0)]
        [Route("pensionregulator/aorn", Order = 1)]
        public async Task<ActionResult> SearchPensionRegulatorByAorn(string payeRef, string aorn, string hashedAccountId)
        {
            if (!string.IsNullOrWhiteSpace(hashedAccountId))
            {
                _accountCookieStorage.Delete(typeof(HashedAccountIdModel).FullName);

                _accountCookieStorage.Create(
                    new HashedAccountIdModel { Value = hashedAccountId },
                    typeof(HashedAccountIdModel).FullName);
            }

            var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var aornLock = await _mediatr.SendAsync(new GetUserAornLockRequest
            {
                UserRef = userRef
            });

            if (!string.IsNullOrWhiteSpace(payeRef) && !string.IsNullOrWhiteSpace(aorn))
            {
                return await PerformSearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel
                {
                    Aorn = aorn,
                    PayeRef = payeRef,
                    IsLocked = aornLock.UserAornStatus.IsLocked,
                    RemainingAttempts = aornLock.UserAornStatus.RemainingAttempts,
                    AllowedAttempts = aornLock.UserAornStatus.AllowedAttempts,
                    RemainingLock = aornLock.UserAornStatus.RemainingLock
                });
            }

            return View(ControllerConstants.SearchUsingAornViewName, new SearchPensionRegulatorByAornViewModel
            {
                IsLocked = aornLock.UserAornStatus.IsLocked,
                RemainingAttempts = aornLock.UserAornStatus.RemainingAttempts,
                AllowedAttempts = aornLock.UserAornStatus.AllowedAttempts,
                RemainingLock = aornLock.UserAornStatus.RemainingLock
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{HashedAccountId}/pensionregulator/aorn", Order = 0)]
        [Route("pensionregulator/aorn", Order = 1)]
        public async Task<ActionResult> SearchPensionRegulatorByAorn(SearchPensionRegulatorByAornViewModel viewModel)
        {
            ValidateAndFormatSearchPensionRegulatorByAornViewModel(viewModel);

            if (!viewModel.Valid)
            {
                return View(ControllerConstants.SearchUsingAornViewName, viewModel);
            }

            return await PerformSearchPensionRegulatorByAorn(viewModel);
        }

        private async Task<ActionResult> PerformSearchPensionRegulatorByAorn(SearchPensionRegulatorByAornViewModel viewModel)
        {
            var userRef = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var model = await _searchPensionRegulatorOrchestrator.GetOrganisationsByAorn(viewModel.Aorn, viewModel.PayeRef);

            await _mediatr.SendAsync(new UpdateUserAornLockRequest
            {
                UserRef = userRef,
                Success = model.Data.Results.Count > 0
            });

            switch (model.Data.Results.Count)
            {
                case 0:
                    {
                        var aornLock = await _mediatr.SendAsync(new GetUserAornLockRequest
                        {
                            UserRef = userRef
                        });

                        viewModel.IsLocked = aornLock.UserAornStatus.IsLocked;
                        viewModel.RemainingAttempts = aornLock.UserAornStatus.RemainingAttempts;
                        viewModel.AllowedAttempts = aornLock.UserAornStatus.AllowedAttempts;
                        viewModel.RemainingLock = aornLock.UserAornStatus.RemainingLock;

                        return View(ControllerConstants.SearchUsingAornViewName, viewModel);
                    }
                case 1:
                    {
                        if (await CheckIfPayeSchemeAlreadyInUse(viewModel.PayeRef))
                        {
                            return RedirectToAction(ControllerConstants.PayeErrorActionName, ControllerConstants.EmployerAccountControllerName, new { NotFound = false });
                        }
                        await SavePensionRegulatorDataIfItHasAValidName(model.Data.Results.First(), true, false, viewModel.Aorn, viewModel.PayeRef);
                        return RedirectToAction(ControllerConstants.SummaryActionName, ControllerConstants.EmployerAccountControllerName);
                    }
                default:
                    {
                        if (await CheckIfPayeSchemeAlreadyInUse(viewModel.PayeRef))
                        {
                            return RedirectToAction(ControllerConstants.PayeErrorActionName, ControllerConstants.EmployerAccountControllerName, new { NotFound = false });
                        }
                        await SavePayeDetails(viewModel.Aorn, viewModel.PayeRef);
                        return View(ControllerConstants.SearchPensionRegulatorResultsViewName, model.Data);
                    }
            }
        }

        private void ValidateAndFormatSearchPensionRegulatorByAornViewModel(SearchPensionRegulatorByAornViewModel viewModel)
        {
            var errors = new Dictionary<string, string>();

            viewModel.Aorn = viewModel.Aorn?.ToUpper().Trim();
            viewModel.PayeRef = viewModel.PayeRef?.ToUpper().Trim();

            if (string.IsNullOrWhiteSpace(viewModel.Aorn))
            {
                errors.Add(nameof(viewModel.Aorn), "Enter your reference number to continue");
            }
            else if (!_aornRegex.IsMatch(viewModel.Aorn.ToUpper().Trim()))
            {
                errors.Add(nameof(viewModel.Aorn), "Enter an accounts office reference number in the correct format");
            }

            if (string.IsNullOrWhiteSpace(viewModel.PayeRef))
            {
                errors.Add(nameof(viewModel.PayeRef), "Enter your PAYE scheme to continue");
            }
            else if (!_payeRegex.IsMatch(viewModel.PayeRef))
            {
                errors.Add(nameof(viewModel.PayeRef), "Enter a PAYE scheme number in the correct format");
            }
            else if (viewModel.PayeRef[3] != '/')
            {
                viewModel.PayeRef = viewModel.PayeRef.Insert(3, "/");
            }

            viewModel.AddErrorsFromDictionary(errors);
        }

        private async Task<bool> CheckIfPayeSchemeAlreadyInUse(string empRef)
        {
            var schemeCheck = await _mediatr.SendAsync(new GetPayeSchemeInUseQuery { Empref = empRef });

            return schemeCheck.PayeScheme != null;
        }

        private async Task SavePayeDetails(string aorn, string payeRef)
        {
            await _mediatr.SendAsync(new SavePayeRefData(new EmployerAccountPayeRefData
            {
                PayeReference = payeRef,
                AORN = aorn
            }));
        }

        private async Task SavePensionRegulatorOrganisationDataIfItHasAValidName(
            PensionRegulatorDetailsViewModel viewModel, bool newSearch, bool multipleResults)
        {
            if (viewModel?.Name != null)
            {
                await _mediatr
                    .SendAsync(new SaveOrganisationData
                    (
                        new EmployerAccountOrganisationData
                        {
                            OrganisationReferenceNumber = viewModel.ReferenceNumber.ToString(),
                            OrganisationName = viewModel.Name,
                            OrganisationType = viewModel.Type,
                            OrganisationRegisteredAddress = viewModel.Address,
                            OrganisationStatus = viewModel.Status ?? string.Empty,
                            NewSearch = newSearch,
                            PensionsRegulatorReturnedMultipleResults = multipleResults
                        }
                    ));
            }
        }

        private async Task SavePensionRegulatorDataIfItHasAValidName(PensionRegulatorDetailsViewModel viewModel, bool newSearch, bool multipleResults, string aorn, string payeRef)
        {
            if (viewModel?.Name != null)
            {
                await _mediatr
                    .SendAsync(new SaveOrganisationAndPayeData
                    (
                        new EmployerAccountOrganisationData
                        {
                            OrganisationReferenceNumber = viewModel.ReferenceNumber.ToString(),
                            OrganisationName = viewModel.Name,
                            OrganisationType = viewModel.Type,
                            OrganisationRegisteredAddress = viewModel.Address,
                            OrganisationStatus = viewModel.Status ?? string.Empty,
                            NewSearch = newSearch,
                            PensionsRegulatorReturnedMultipleResults = multipleResults
                        },
                        new EmployerAccountPayeRefData { AORN = aorn, PayeReference = payeRef }
                    ));
            }
        }
    }
}