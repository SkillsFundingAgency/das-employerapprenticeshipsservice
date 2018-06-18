using System;
using System.Linq;
using Microsoft.Azure;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    public class CurrentDatePolicy : ConfiguredInstancePolicy
    {
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var currentDateTime = instance?.Constructor?.GetParameters().FirstOrDefault(p => p.ParameterType == typeof(ICurrentDateTime));

            if (currentDateTime != null)
            {
                var cloudCurrentTime = CloudConfigurationManager.GetSetting("CurrentTime");

                if (!DateTime.TryParse(cloudCurrentTime, out var currentTime))
                {
                    currentTime = DateTime.Now;
                }

                instance.Dependencies.AddForConstructorParameter(currentDateTime, new CurrentDateTime(currentTime));
            }
        }
    }
}