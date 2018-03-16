﻿using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    /// <summary>
    ///     Represents a service that can determine whether an operation (represented by a controller name and action name) are 
    ///     available in a particular membership context.
    /// </summary>
    /// <remarks>
    ///     The distinction between this and <see cref="IFeatureToggleService"/> is that this service will perform
    ///     whatever checks are defined to determine when an operation is available whereas <see cref="IFeatureToggleService"/>
    ///     only performs a whitelist check, which is just one specific check made by this service.
    /// </remarks>
    public interface IOperationAuthorisationService
    {
        bool IsOperationAuthorised(string controllerName, string actionName, IAuthorizationContext authorisationContext);
    }
}
