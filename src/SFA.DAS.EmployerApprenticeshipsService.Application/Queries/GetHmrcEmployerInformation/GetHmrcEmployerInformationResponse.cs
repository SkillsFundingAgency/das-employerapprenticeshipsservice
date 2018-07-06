using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationResponse
    {
        public HMRC.ESFA.Levy.Api.Types.EmpRefLevyInformation EmployerLevyInformation { get; set; }
        public string Empref { get; set; }
        public bool EmprefNotFound { get; set; }
    }
}