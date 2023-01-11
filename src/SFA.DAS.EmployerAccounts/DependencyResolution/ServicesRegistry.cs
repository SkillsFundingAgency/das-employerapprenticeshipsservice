using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class ServicesRegistry : Registry
{
    public ServicesRegistry()
    {            
        For<IRestClientFactory>().Use<RestClientFactory>();
        For<IRestServiceFactory>().Use<RestServiceFactory>();
        For<IHttpServiceFactory>().Use<HttpServiceFactory>();
        For<IUserAornPayeLockService>().Use<UserAornPayeLockService>();
        For<IReservationsService>().Use<ReservationsService>();
        For<IReservationsService>().DecorateAllWith<ReservationsServiceWithTimeout>();
        For<ICommitmentV2Service>().Use<CommitmentsV2Service>();
        For<ICommitmentV2Service>().DecorateAllWith<CommitmentsV2ServiceWithTimeout>();
    }
}