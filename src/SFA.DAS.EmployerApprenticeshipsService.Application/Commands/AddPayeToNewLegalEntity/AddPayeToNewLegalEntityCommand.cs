using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity
{
    public class AddPayeToNewLegalEntityCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
        public string Empref { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityAddress { get; set; }
        public string LegalEntityName { get; set; }
        public DateTime LegalEntityDate { get; set; }
    }
}
