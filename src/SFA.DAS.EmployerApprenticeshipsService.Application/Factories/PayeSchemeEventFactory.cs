using System.Web;
using SFA.DAS.EAS.Account.Api.Types.Events.PayeScheme;

namespace SFA.DAS.EAS.Application.Factories
{
    public class PayeSchemeEventFactory : IPayeSchemeEventFactory
    {
        public PayeSchemeAddedEvent CreatePayeSchemeAddedEvent(string hashedAccountId, string payeSchemeRef)
        {
            return new PayeSchemeAddedEvent
            {
                Event = "PayeSchemeAdded",
                ResourceUri =  GeneratePayeResourceUrl(hashedAccountId, payeSchemeRef)
            };
        }

        public PayeSchemeRemovedEvent CreatePayeSchemeRemovedEvent(string hashedAccountId, string payeSchemeRef)
        {
            return new PayeSchemeRemovedEvent
            {
                Event = "PayeSchemeRemoved",
                ResourceUri = GeneratePayeResourceUrl(hashedAccountId, payeSchemeRef)
            };
        }

        private static string GeneratePayeResourceUrl(string hashedAccountId, string payeSchemeRef)
        {
            // This is deliberately double encoded to minic the response the accounts web 
            // api returns when encoding the URI to json as part of the GetAccounts method.
            var schemeUrlString = HttpUtility.UrlEncode(HttpUtility.UrlEncode(payeSchemeRef));

            var resourceUri = $"api/accounts/{hashedAccountId}/payeschemes/{schemeUrlString}";

            return resourceUri;
        }
    }
}
