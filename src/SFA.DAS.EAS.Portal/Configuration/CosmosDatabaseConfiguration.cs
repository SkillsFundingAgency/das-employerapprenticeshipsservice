
namespace SFA.DAS.EAS.Portal.Configuration
{
    //todo: can we just use ICosmosDbConfiguration instead?
    public class CosmosDatabaseConfiguration //: ICosmosDbConfiguration
    {
        public string Uri { get; set; }
        public string AuthKey { get; set; }
    }
}
