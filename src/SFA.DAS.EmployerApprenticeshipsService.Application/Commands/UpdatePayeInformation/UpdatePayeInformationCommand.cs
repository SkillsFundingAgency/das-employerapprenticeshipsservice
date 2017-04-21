using MediatR;

namespace SFA.DAS.EAS.Application.Commands.UpdatePayeInformation
{
    public class UpdatePayeInformationCommand : IAsyncRequest
    {
        public string PayeRef { get; set; }
    }
}
