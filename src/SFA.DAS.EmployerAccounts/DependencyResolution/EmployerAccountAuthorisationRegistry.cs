﻿using SFA.DAS.Authorization.Handlers;
using SFA.DAS.EmployerAccounts.AuthorisationExtensions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class AuthorisationRegistry : Registry
    {
        public AuthorisationRegistry()
        {
            For<IAuthorizationHandler>().Add<EmployerFeatureAuthorizationHandler>();
            For<IAuthorisationResourceRepository>().Use<AuthorisationResourceRepository>();
        }
    }
}
