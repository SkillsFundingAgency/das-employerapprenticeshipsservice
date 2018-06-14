using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EAS.Application.Messages
{
    [MessageGroup("calculate_transfer_allowance_snapshot")]
    public class CalculateTransferAllowanceSnapshotCommand
    {
        public long AccountId { get; set; }
    }
}
