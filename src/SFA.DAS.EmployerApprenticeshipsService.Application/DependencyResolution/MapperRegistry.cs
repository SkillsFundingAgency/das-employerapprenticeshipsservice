using System;
using System.Linq;
using AutoMapper;
using SFA.DAS.EAS.Domain;
using StructureMap;
using StructureMap.TypeRules;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class MapperRegistry : Registry
    {
        public MapperRegistry()
        {
            var profiles = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName.StartsWith(Constants.ServiceNamespace))
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Profile).IsAssignableFrom(t) && t.IsConcrete() && t.HasConstructors())
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }
    }
}