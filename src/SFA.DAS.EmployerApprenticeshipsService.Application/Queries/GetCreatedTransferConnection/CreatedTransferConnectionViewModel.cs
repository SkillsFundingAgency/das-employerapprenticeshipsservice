using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Application.Commands.SendTransferConnection;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Models.TransferConnection;

namespace SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnection
{
    public class CreatedTransferConnectionViewModel : ViewModel<SendTransferConnectionCommand>
    {
        [Required]
        [Range(1, 2)]
        public int Choice { get; set; }
        
        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public TransferConnection TransferConnection { get; set; }

        protected override SendTransferConnectionCommand Map()
        {
            return new SendTransferConnectionCommand
            {
                TransferConnectionId = TransferConnection.Id
            };
        }
    }
}