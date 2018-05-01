using System;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class RemovePayeSchemeViewModel :ViewModelBase
    {
        public Guid ExternalUserId { get; set; }
        public string HashedAccountId { get; set; }
        public string PayeRef { get; set; }
        public string PayeSchemeName { get; set; }
        public string AccountName { get; set; }
        public int RemoveScheme { get; set; }

        public string RemoveSchemeErrorMessage => GetErrorMessage(nameof(RemoveScheme));
    }
}