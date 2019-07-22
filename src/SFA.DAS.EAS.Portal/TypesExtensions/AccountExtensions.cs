using System;
using System.Linq;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.TypesExtensions
{
    public static class AccountExtensions
    {
        public static Organisation GetOrAddOrganisation(this Account account, long accountLegalEntityId, Action<Organisation> onAdd = null, Action<Organisation> onGet = null)
        {
            var organisation = account.Organisations.SingleOrDefault(o => o.AccountLegalEntityId == accountLegalEntityId);
            if (organisation == null)
            {
                organisation = new Organisation {AccountLegalEntityId = accountLegalEntityId};
                account.Organisations.Add(organisation);
                onAdd?.Invoke(organisation);
            }
            else
            {
                onGet?.Invoke(organisation);
            }

            return organisation;
        }

        public static Provider GetOrAddProvider(this Account account, long ukprn, Action<Provider> onAdd = null, Action<Provider> onGet = null)
        {
            var provider = account.Providers.SingleOrDefault(p => p.Ukprn == ukprn);
            if (provider == null)
            {
                provider = new Provider {Ukprn = ukprn};
                account.Providers.Add(provider);
                onAdd?.Invoke(provider);
            }
            else
            {
                onGet?.Invoke(provider);
            }

            return provider;
        }
    }
}