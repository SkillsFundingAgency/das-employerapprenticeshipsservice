using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionQuery : IAsyncRequest<GetEmployerEnglishFractionResponse>
    {
        public string EmpRef { get; set; }
        public Guid ExternalUserId { get; set; }
        public string HashedAccountId { get; set; }
    }
}