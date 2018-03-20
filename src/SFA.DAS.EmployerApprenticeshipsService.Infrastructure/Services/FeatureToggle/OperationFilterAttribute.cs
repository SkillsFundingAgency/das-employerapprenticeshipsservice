using System;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.Services.FeatureToggle
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class OperationFilterAttribute : Attribute
    {
        /// <summary>
        ///     A feature that controls access to the operation.
        /// </summary>
        public Feature RequiresAccessToFeature { get; set; }
    }
}