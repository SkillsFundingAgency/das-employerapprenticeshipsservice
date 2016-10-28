using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAgreementTemplate
{
    public class CreateEmployerAgreementTemplateCommand : IAsyncRequest
    {
        public string TemplateRef { get; set; }
        public string Text { get; set; }
    }
}