using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.UpdateEnglishFractions
{
    public class UpdateEnglishFractionsCommand : IAsyncRequest
    {
        public string EmployerReference { get; set; }
        public string AuthToken { get; set; }
    }
}
