using System.Threading.Tasks;
using System.Web;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountOrchestrator : HmrcOrchestratorBase
    {
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";
        
        public EmployerAccountOrchestrator(IMediator mediator, ILogger logger, ICookieService cookieService) : base(mediator,logger,cookieService)
        {
        }
        
        public async Task<SelectEmployerViewModel> GetCompanyDetails(SelectEmployerModel model)
        {
            var response = await Mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = model.EmployerRef
            });
            
            if (response == null)
            {
                Logger.Warn("No response from SelectEmployerViewModel");
                return new SelectEmployerViewModel();
            }

            Logger.Info($"Returning Data for {model.EmployerRef}");

            return new SelectEmployerViewModel
            {
                CompanyNumber = response.CompanyNumber,
                CompanyName = response.CompanyName,
                DateOfIncorporation = response.DateOfIncorporation,
                RegisteredAddress = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}"
            };
        }

        public async Task CreateAccount(CreateAccountModel model)
        {
            await Mediator.SendAsync(new CreateAccountCommand
            {
                UserId = model.UserId,
                CompanyNumber = model.CompanyNumber,
                CompanyName = model.CompanyName,
                EmployerRef = model.EmployerRef
            });
        }

        public EmployerAccountData GetCookieData(HttpContextBase context)
        {
            var cookie = (string)CookieService.Get(context, CookieName);

            return JsonConvert.DeserializeObject<EmployerAccountData>(cookie);
        }

        public void CreateCookieData(HttpContextBase context, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            CookieService.Create(context, CookieName, json, 365);
        }

        public void UpdateCookieData(HttpContextBase context, object data)
        {
            CookieService.Update(context, CookieName, JsonConvert.SerializeObject(data));
        }
        
    }
}