using AutoMapper;
using StructureMap;
using StructureMap.TypeRules;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class MapperRegistry : Registry
    {
        public MapperRegistry()
        {
            try
            {

                var profiles = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => a.FullName.StartsWith("SFA.DAS.EmployerFinance"))
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
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
            }
        }

    }
}