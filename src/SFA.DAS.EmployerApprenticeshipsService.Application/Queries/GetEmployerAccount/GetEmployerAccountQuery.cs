using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        [Required]
        public long AccountId { get; set; }

        public Guid ExternalUserId { get; set; }
    }
}
