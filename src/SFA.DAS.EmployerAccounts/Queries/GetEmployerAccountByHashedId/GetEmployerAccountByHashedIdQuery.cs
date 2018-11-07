using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountByHashedId
{
    public class GetEmployerAccountByHashedIdQuery : IAsyncRequest<GetEmployerAccountByHashedIdResponse>
    {
        [Required]
        public string HashedAccountId { get; set; }
    }
}