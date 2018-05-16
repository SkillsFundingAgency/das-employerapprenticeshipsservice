namespace SFA.DAS.EAS.Account.Worker.UnitTests.TestFixtures
{
    public class BatchInfo
    {
        public BatchInfo(long startAfterId, int count)
        {
            StartAfterId = startAfterId;
            Count = count;
        }
        public long StartAfterId { get; }
        public int Count { get; }
    }
}