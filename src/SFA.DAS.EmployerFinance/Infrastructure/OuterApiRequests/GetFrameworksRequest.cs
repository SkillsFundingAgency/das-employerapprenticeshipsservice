using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetFrameworksRequest: IGetApiRequest
    {
        public string GetUrl => "TrainingCourses/frameworks";
    }
}