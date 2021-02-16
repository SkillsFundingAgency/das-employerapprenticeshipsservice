using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetStandardsRequest : IGetApiRequest
    {
        public string GetUrl => "TrainingCourses/standards";
    }
}