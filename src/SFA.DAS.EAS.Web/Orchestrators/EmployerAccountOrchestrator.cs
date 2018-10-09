using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetLatestAccountAgreementTemplate;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAccountOrchestrator : EmployerVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IHashingService _hashingService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        //Needed for tests
        protected EmployerAccountOrchestrator()
        {
        }

        public EmployerAccountOrchestrator(IMediator mediator, ILog logger, ICookieStorageService<EmployerAccountData> cookieService,
            EmployerApprenticeshipsServiceConfiguration configuration, IHashingService hashingService)
            : base(mediator, logger, cookieService, configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _hashingService = hashingService;
        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateAccount(CreateAccountViewModel viewModel, HttpContextBase context)
        {
            try
            {
                var result = await Mediator.SendAsync(new CreateAccountCommand
                {
                    ExternalUserId = viewModel.UserId,
                    OrganisationType = viewModel.OrganisationType,
                    OrganisationName = viewModel.OrganisationName,
                    OrganisationReferenceNumber = viewModel.OrganisationReferenceNumber,
                    OrganisationAddress = viewModel.OrganisationAddress,
                    OrganisationDateOfInception = viewModel.OrganisationDateOfInception,
                    PayeReference = viewModel.PayeReference,
                    AccessToken = viewModel.AccessToken,
                    RefreshToken = viewModel.RefreshToken,
                    OrganisationStatus = viewModel.OrganisationStatus,
                    EmployerRefName = viewModel.EmployerRefName,
                    PublicSectorDataSource = viewModel.PublicSectorDataSource,
                    Sector = viewModel.Sector
                });

                CookieService.Delete(CookieName);

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            HashedAccountId = result.HashedAccountId
                        }
                    },
                    Status = HttpStatusCode.OK
                };
            }
            catch (InvalidRequestException ex)
            {
                Logger.Info($"Create Account Validation Error: {ex.Message}");
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel(),
                    Status = HttpStatusCode.BadRequest,
                    Exception = ex,
                    FlashMessage = new FlashMessageViewModel()
                };
            }

        }


        public virtual OrchestratorResponse<SummaryViewModel> GetSummaryViewModel(HttpContextBase context)
        {
            var enteredData = GetCookieData(context);

            var model = new SummaryViewModel
            {
                OrganisationType = enteredData.OrganisationType,
                OrganisationName = enteredData.OrganisationName,
                RegisteredAddress = enteredData.OrganisationRegisteredAddress,
                OrganisationReferenceNumber = enteredData.OrganisationReferenceNumber,
                OrganisationDateOfInception = enteredData.OrganisationDateOfInception,
                PayeReference = enteredData.PayeReference,
                EmployerRefName = enteredData.EmployerRefName,
                EmpRefNotFound = enteredData.EmpRefNotFound,
                OrganisationStatus = enteredData.OrganisationStatus,
                PublicSectorDataSource = enteredData.PublicSectorDataSource,
                Sector = enteredData.Sector,
                NewSearch = enteredData.NewSearch
            };

            return new OrchestratorResponse<SummaryViewModel>
            {
                Data = model
            };

        }


        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            return CookieService.Get(CookieName);

        }

        public virtual void CreateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            CookieService.Create(data, CookieName, 365);
        }

        public void UpdateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            CookieService.Update(CookieName, data);
        }

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetAccountAgreementTemplate(CreateAccountViewModel viewModel)
        {
            var response = new OrchestratorResponse<EmployerAgreementViewModel>();

            var templateResponse = await Mediator.SendAsync(new GetLatestAccountAgreementTemplateRequest());

            response.Data = new EmployerAgreementViewModel
            {
                EmployerAgreement = new EmployerAgreementView
                {
                    LegalEntityName = viewModel.OrganisationName,
                    LegalEntityCode = viewModel.OrganisationReferenceNumber,
                    LegalEntitySource = viewModel.OrganisationType,
                    LegalEntityAddress = viewModel.OrganisationAddress,
                    LegalEntityInceptionDate = viewModel.OrganisationDateOfInception,
                    Status = EmployerAgreementStatus.Pending,
                    TemplatePartialViewName = templateResponse.Template.PartialViewName
                }
            };

            return response;
        }

        public virtual void DeleteCookieData(HttpContextBase context)
        {
            CookieService.Delete(CookieName);
        }

        public async Task<OrchestratorResponse<EmployerAccountViewModel>> GetEmployerAccount(string hashedAccountId)
        {
            var response = await Mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedAccountId
            });

            return new OrchestratorResponse<EmployerAccountViewModel>
            {
                Data = new EmployerAccountViewModel
                {
                    HashedId = hashedAccountId,
                    Name = response.Account.Name
                }
            };
        }
    }
}