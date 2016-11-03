﻿namespace SFA.DAS.EAS.Web.Models
{
    public class AcknowledgementViewModel
    {
        public string HashedCommitmentId { get; set; }
        public string ProviderName { get; set; }
        public string LegalEntityName { get; set; }
        public string Message { get; set; }
    }
}