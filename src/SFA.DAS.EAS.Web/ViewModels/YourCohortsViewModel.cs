namespace SFA.DAS.EAS.Web.ViewModels
{
    public class YourCohortsViewModel
    {
        public int WaitingToBeSentCount { get; set; }

        public int ReadyForApprovalCount { get; set; }

        public int ReadyForReviewCount { get; set; }

        public int WithProviderCount { get; set; }
    }
}