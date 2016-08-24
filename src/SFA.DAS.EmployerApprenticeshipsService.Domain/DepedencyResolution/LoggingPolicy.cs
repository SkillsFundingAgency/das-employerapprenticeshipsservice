using System;
using System.Linq;
using NLog;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.DepedencyResolution
{
    public class LoggingPolicy : ConfiguredInstancePolicy
    {
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var logger = instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(ILogger));

            if (logger != null)
            {
                instance.Dependencies.AddForConstructorParameter(logger, LogManager.GetLogger(pluginType.FullName));
            }

        }
    }
}