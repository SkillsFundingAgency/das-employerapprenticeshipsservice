using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ReleaseEmployerAgreementTemplate
{
    public class ReleaseEmployerAgreementTemplateCommand : IAsyncRequest
    {
        public int TemplateId { get; set; }
    }
}