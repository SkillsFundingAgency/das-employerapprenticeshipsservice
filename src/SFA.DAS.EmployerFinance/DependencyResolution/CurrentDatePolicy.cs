using Microsoft.Azure;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Time;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Linq;

namespace SFA.DAS.EmployerFinance.DependencyResolution
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