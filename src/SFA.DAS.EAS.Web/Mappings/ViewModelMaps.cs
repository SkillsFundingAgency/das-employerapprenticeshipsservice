using System.Linq;
using AutoMapper;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class ViewModelMaps : Profile
    {
        public ViewModelMaps()
        {
            var types = GetType()
                .Assembly
                .GetTypes()
                .Where(t =>
                    t.BaseType != null &&
                    t.BaseType.IsGenericType &&
                    t.BaseType.GetGenericTypeDefinition() == typeof(ViewModel<>)
                );

            foreach (var type in types)
            {
                CreateMap(type, type.BaseType.GetGenericArguments().Single());
            }
        }
    }
}