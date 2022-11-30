using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.TrainingCourses
{
    public class GetFrameworksRequest: IGetApiRequest
    {
        public string GetUrl => "trainingCourses/frameworks";
    }
}