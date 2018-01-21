using SFA.DAS.EAS.Application.Commands.CreateTransferConnection;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class CreateTransferConnectionViewModel : ViewModel<CreateTransferConnectionCommand>
    {
        public string SenderHashedAccountId { get; set; }

        protected override CreateTransferConnectionCommand Map()
        {
            return new CreateTransferConnectionCommand
            {
                SenderHashedAccountId = SenderHashedAccountId
            };
        }
    }
}