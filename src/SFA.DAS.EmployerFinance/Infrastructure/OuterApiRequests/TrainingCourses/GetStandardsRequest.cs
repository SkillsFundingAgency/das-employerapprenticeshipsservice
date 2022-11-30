using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.TrainingCourses
{
    public class GetStandardsRequest : IGetApiRequest
    {
        public string GetUrl => "trainingCourses/standards";
    }
}