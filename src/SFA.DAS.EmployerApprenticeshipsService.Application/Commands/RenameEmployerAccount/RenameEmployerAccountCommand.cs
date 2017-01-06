using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public string ExternalUserId { get; set; }
        public string NewName { get; set; }
    }
}
