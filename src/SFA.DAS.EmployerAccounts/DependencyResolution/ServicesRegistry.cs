using System.Net;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ServicesRegistry : Registry
    {
        public ServicesRegistry()
        {
            For<IAddressLookupService>().Use<AddressLookupService>();
            For<IRestClientFactory>().Use<RestClientFactory>();
            For<IRestServiceFactory>().Use<RestServiceFactory>();         
            For<IHttpServiceFactory>().Use<HttpServiceFactory>();
            For<IUserAornPayeLockService>().Use<UserAornPayeLockService>();
            var reservationServiceInstance = For<IReservationsService>().Use<ReservationsService>();
            //For<IReservationsService>().Use<ReservationsServiceWithTimeout>()
            //    .Ctor<IReservationsService>().Is(reservationServiceInstance);
            For<IReservationsService>().Use<ReservationsService>();
            For<ICommitmentV2Service>().Use<CommitmentsV2Service>();
        }
    }
}