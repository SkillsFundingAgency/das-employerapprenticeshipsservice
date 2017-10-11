using System;
using System.Linq;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.EnvironmentInfo;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    
    public class ConfigurationPolicy<T> : ConfiguredInstancePolicy where T : IConfiguration
    {
        private readonly string _serviceName;
        private readonly IConfigurationInfo<T> _configInfo;

        public ConfigurationPolicy(string serviceName, IConfigurationInfo<T> configInfo)
        {
            _serviceName = serviceName;
            _configInfo= configInfo;
        }
        
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {

            var serviceConfigurationParamater = instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(T) 
                                                                                                        || ((System.Reflection.TypeInfo)typeof(T)).GetInterface(x.ParameterType.Name) != null);

            if (serviceConfigurationParamater != null)
            {
                var result = _configInfo.GetConfiguration(_serviceName, null);
                if (result != null)
                {
                    instance.Dependencies.AddForConstructorParameter(serviceConfigurationParamater, result);
                }
            }
            
        }
    }
}