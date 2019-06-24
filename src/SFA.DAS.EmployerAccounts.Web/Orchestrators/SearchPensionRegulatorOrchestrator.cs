﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class SearchPensionRegulatorOrchestrator : UserVerificationOrchestratorBase 
    {
        private readonly ILog _logger;
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        protected SearchPensionRegulatorOrchestrator()
        {
        }

        public SearchPensionRegulatorOrchestrator(IMediator mediator, ICookieStorageService<EmployerAccountData> cookieService, ILog logger)
            : base(mediator)
        {
            _cookieService = cookieService;
            _logger = logger;
        }

        public virtual async Task<OrchestratorResponse<SearchPensionRegulatorResultsViewModel>> SearchPensionRegulator(string payeRef)
        {
            var response = new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>() { Data = new SearchPensionRegulatorResultsViewModel { PayeRef = payeRef } };

            try
            {
                var result = await Mediator.SendAsync(new GetPensionRegulatorRequest { PayeRef = payeRef });
                response.Data.Results = CreateResult(result.Organisations).ToList();
            }            
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.Data.Results = new List<PensionRegulatorDetailsViewModel>();
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<SearchPensionRegulatorResultsViewModel>> GetOrganisationsByAorn(string aorn, string payeRef)
        {
            var response = new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>() { Data = new SearchPensionRegulatorResultsViewModel { Aorn = aorn, PayeRef = payeRef } };

            try
            {
                var result = await Mediator.SendAsync(new GetOrganisationsByAornRequest(aorn, payeRef));
                response.Data.Results = CreateResult(result.Organisations).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.Data.Results = new List<PensionRegulatorDetailsViewModel>();
            }

            return response;
        }


        public virtual EmployerAccountData GetCookieData()
        {
            return _cookieService.Get(CookieName);
        }                

        private IEnumerable<PensionRegulatorDetailsViewModel> CreateResult(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(ConvertToViewModel);
        }
        
        private PensionRegulatorDetailsViewModel ConvertToViewModel(Organisation organisation)
        {
            return new PensionRegulatorDetailsViewModel
            {
                Name = organisation.Name,
                Status = organisation.Status,
                Address = organisation.Address.FormatAddress(),
                ReferenceNumber = organisation.UniqueIdentity,
                Type = OrganisationType.PensionsRegulator
            };
        }      
    }
}