using NLog.Time;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers;
using SFA.DAS.EAS.Infrastructure.Services.Features;
using StructureMap;


namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    public class AuthorisationRegistry : Registry
    {
        public AuthorisationRegistry()
        {
            For<IAuthorizationHandler[]>().Use(ctx => new IAuthorizationHandler[]
            {
                // The order of the types specified here is the order in which the handlers will be executed. 
                ctx.GetInstance<FeatureEnabledAuthorisationHandler>(),
                ctx.GetInstance<FeatureWhitelistAuthorisationHandler>(),
                ctx.GetInstance<AgreementFeatureAuthorisationHandler>()
            }).Singleton();

            For<IFeatureService>().Use<FeatureService>().Singleton();
            For<IFeatureCache>().Use<FeatureCache>().Singleton();
            For<IAccountAgreementService>().Use<AccountAgreementService>().Singleton();
        }
    }
}