using System;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    /// <summary>
    ///     Attach to either a controller or controller method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class OperationFilterAttribute : Attribute
    {
        /// <summary>
        ///     Get or sets the feature that this operation is part of. 
        /// </summary>
        public FeatureType RequiresAccessToFeatureType { get; set; }
    }
}