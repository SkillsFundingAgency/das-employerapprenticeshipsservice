using System;
using System.Linq;
using SFA.DAS.EAS.Domain.Interfaces;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    
    public class ConfigurationPolicy<T> : ConfiguredInstancePolicy where T : IConfiguration
    {
        private readonly string _serviceName;

        public ConfigurationPolicy(string serviceName)
        {
            _serviceName = serviceName;
        }
        
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {

            var serviceConfigurationParamater = instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(T) 
                                                                                                        || ((System.Reflection.TypeInfo)typeof(T)).GetInterface(x.ParameterType.Name) != null);

            if (serviceConfigurationParamater != null)
            {
                var result = ConfigurationHelper.GetConfiguration<T>(_serviceName);
                if (result != null)
                {
                    instance.Dependencies.AddForConstructorParameter(serviceConfigurationParamater, result);
                }
            }
            
        }
    }
}