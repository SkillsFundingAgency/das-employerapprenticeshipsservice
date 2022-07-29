using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccountDetail
{
    public class GetEmployerAccountDetailByHashedIdQuery : IAsyncRequest<GetEmployerAccountDetailByHashedIdResponse>
    {
        [Required]
        public string HashedAccountId { get; set; }
    }
}