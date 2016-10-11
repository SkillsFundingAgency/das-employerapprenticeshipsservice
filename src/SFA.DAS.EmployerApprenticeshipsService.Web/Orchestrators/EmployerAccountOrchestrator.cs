using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestAccountAgreementTemplate;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountOrchestrator : EmployerVerificationOrchestratorBase
    {
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        //Needed for tests
        protected EmployerAccountOrchestrator()
        {

        }

        public EmployerAccountOrchestrator(IMediator mediator, ILogger logger, ICookieService cookieService,
            EmployerApprenticeshipsServiceConfiguration configuration)
            : base(mediator, logger, cookieService, configuration)
        {

        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateAccount(CreateAccountModel model, HttpContextBase context)
        {
            if (!model.UserIsAuthorisedToSign && model.SignedAgreement)
            {
                var response = await GetAccountAgreementTemplate(model);
                response.Status = HttpStatusCode.Unauthorized;

                return response;
            }
            try
            {
                var result = await Mediator.SendAsync(new CreateAccountCommand
                {
                    ExternalUserId = model.UserId,
                    CompanyNumber = model.CompanyNumber,
                    CompanyName = model.CompanyName,
                    CompanyRegisteredAddress = model.CompanyRegisteredAddress,
                    CompanyDateOfIncorporation = model.CompanyDateOfIncorporation,
                    EmployerRef = model.EmployerRef,
                    AccessToken = model.AccessToken,
                    RefreshToken = model.RefreshToken,
                    SignAgreement = model.UserIsAuthorisedToSign && model.SignedAgreement
                });

                CookieService.Delete(context, CookieName);

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            HashedId = result.HashedId
                        }
                    },
                    Status = HttpStatusCode.OK
                };
            }
            catch (InvalidRequestException ex)
            {
                Logger.Info(ex,"Create Account Validation Error");
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel(),
                    Status = HttpStatusCode.BadRequest,
                    Exception = ex,
                    FlashMessage = new FlashMessageViewModel()
                };
            }
           
        }

        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            var cookie = (string)CookieService.Get(context, CookieName);

            if(string.IsNullOrEmpty(cookie))
                return null;
            
            return JsonConvert.DeserializeObject<EmployerAccountData>(cookie);
        }

        public virtual void CreateCookieData(HttpContextBase context, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            CookieService.Create(context, CookieName, json, 365);
        }

        public void UpdateCookieData(HttpContextBase context, object data)
        {
            CookieService.Update(context, CookieName, JsonConvert.SerializeObject(data));
        }

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetAccountAgreementTemplate(CreateAccountModel model)
        {
            var response = new OrchestratorResponse<EmployerAgreementViewModel>();

            var templateResponse = await Mediator.SendAsync(new GetLatestAccountAgreementTemplateRequest());

            response.Data = new EmployerAgreementViewModel
            {
                EmployerAgreement = new EmployerAgreementView
                {
                    LegalEntityName = model.CompanyName,
                    LegalEntityCode = model.CompanyNumber,
                    LegalEntityRegisteredAddress = model.CompanyRegisteredAddress,
                    LegalEntityIncorporatedDate = model.CompanyDateOfIncorporation,
                    Status = EmployerAgreementStatus.Pending,
                    TemplateRef = templateResponse.Template.Ref,
                    TemplateText = templateResponse.Template.Text
                }
            };

            return response;
        }

        public virtual void DeleteCookieData(HttpContextBase context)
        {
            CookieService.Delete(context,CookieName);
        }
    }
}