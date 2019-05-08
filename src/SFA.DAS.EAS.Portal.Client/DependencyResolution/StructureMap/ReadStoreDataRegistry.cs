using Microsoft.Azure.Documents;
using SFA.DAS.EAS.Portal.Client.Data;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class ReadStoreDataRegistry : Registry
    {
        //todo: rename?
        public ReadStoreDataRegistry()
        {
            For<IDocumentClient>().Add(c => c.GetInstance<IDocumentClientFactory>().CreateDocumentClient()).Named(GetType().FullName).Singleton();
            For<IDocumentClientFactory>().Use<DocumentClientFactory>();
            For<IAccountsReadOnlyRepository>().Use<AccountsReadOnlyRepository>().Ctor<IDocumentClient>().IsNamedInstance(GetType().FullName).Singleton();
            For<IPortalClient>().Use<PortalClient>().Singleton();
        }
    }
}