using System;
using System.Linq;
using System.Reflection;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Infrastructure.DependencyResolution
{
    public class ConfigurationPolicy<T> : ConfiguredInstancePolicy where T : class
    {
        private readonly string _serviceName;

        public ConfigurationPolicy(string serviceName)
        {
            _serviceName = serviceName;
        }
        
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var serviceConfigurationParamater = instance?.Constructor?.GetParameters().FirstOrDefault(p => 
                p.ParameterType == typeof(T) || ((TypeInfo)typeof(T)).GetInterface(p.ParameterType.Name) != null);

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