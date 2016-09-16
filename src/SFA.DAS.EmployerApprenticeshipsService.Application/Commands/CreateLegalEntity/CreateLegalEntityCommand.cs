using System;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommand : IAsyncRequest<CreateLegalEntityCommandResponse>
    {
        public long AccountId { get; set; }
        
        public LegalEntity LegalEntity { get; set; }

        public bool SignAgreement { get; set; }

        public DateTime SignedDate { get; set; }

        public string ExternalUserId { get; set; }
    }
}
