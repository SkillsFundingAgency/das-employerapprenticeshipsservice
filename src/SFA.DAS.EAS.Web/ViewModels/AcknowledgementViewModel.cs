namespace SFA.DAS.EAS.Web.ViewModels
{
    public class AcknowledgementViewModel
    {
        public string HashedAccount { get; set; }

        public string HashedCommitmentId { get; set; }

        public string ProviderName { get; set; }

        public string LegalEntityName { get; set; }

        public string Message { get; set; }

        public LinkViewModel BackLink { get; set; }

        public AcknowledgementContent Content { get; set; }
    }

    public class LinkViewModel
    {
        public string Url { get; set; }

        public string  Text { get; set; }
    }

    public class AcknowledgementContent
    {
        public string Title { get; set; }

        public string Text { get; set; }
    }
}