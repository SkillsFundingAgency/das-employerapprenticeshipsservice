using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionQuery : IAsyncRequest<GetEmployerEnglishFractionResponse>
    {
        public string EmpRef { get; set; }
        public string UserId { get; set; }
        public string AccountId { get; set; }
    }
}