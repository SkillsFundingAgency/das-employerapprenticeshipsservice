using MediatR;

namespace SFA.DAS.EAS.Application.Commands.ReleaseEmployerAgreementTemplate
{
    public class ReleaseEmployerAgreementTemplateCommand : IAsyncRequest
    {
        public int TemplateId { get; set; }
    }
}