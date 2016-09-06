using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public interface IEmployerAgreementOrchestrator
    {
        Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(long accountId, string externalUserId);
        Task<OrchestratorResponse<EmployerAgreementViewModel>> GetById(long agreementid, long accountId, string externalUserId);
        Task<OrchestratorResponse> SignAgreement(long agreementid, long accountId, string externalUserId);
        Task<OrchestratorResponse<FindOrganisationViewModel>> FindLegalEntity(long accountId, string companyNumber);
    }
}