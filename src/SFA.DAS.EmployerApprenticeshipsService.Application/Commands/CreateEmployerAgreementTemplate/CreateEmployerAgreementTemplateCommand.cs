using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAgreementTemplate
{
    public class CreateEmployerAgreementTemplateCommand : IAsyncRequest
    {
        public string Text { get; set; }
    }
}