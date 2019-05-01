
namespace SFA.DAS.EAS.Portal.Database
{
    // provider permissions has this in its api.client project, but if the only consumers are the worker & web, then might as well live here
    public static class DocumentSettings
    {
        public const string DatabaseName = "SFA.DAS.EAS.Portal.Database";
        public const string AccountsCollectionName = "Accounts";
    }
}
