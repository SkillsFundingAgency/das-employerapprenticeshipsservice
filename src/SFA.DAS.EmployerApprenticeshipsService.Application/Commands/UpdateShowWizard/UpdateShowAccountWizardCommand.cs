using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.UpdateShowWizard
{
    public class UpdateShowAccountWizardCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
        public bool ShowWizard { get; set; }
    }
}
