using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandResponse
    {
        public EmployerAgreementView AgreementView { get; set; }
    }
}
