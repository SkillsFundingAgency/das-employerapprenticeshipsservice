using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.UpdatePayeInformation
{
    public class UpdatePayeInformationCommand : IAsyncRequest
    {
        public string PayeRef { get; set; }
    }
}
