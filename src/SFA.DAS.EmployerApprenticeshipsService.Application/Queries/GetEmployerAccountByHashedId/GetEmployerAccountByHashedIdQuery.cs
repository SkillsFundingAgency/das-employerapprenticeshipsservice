using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId
{
    public class GetEmployerAccountByHashedIdQuery : IAsyncRequest<GetEmployerAccountByHashedIdResponse>
    {
        [Required]
        public string HashedAccountId { get; set; }
    }
}