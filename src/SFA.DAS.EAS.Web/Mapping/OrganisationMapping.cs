using AutoMapper;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Mapping
{
    public class OrganisationMapping : Profile
    {
        public OrganisationMapping()
        {
            CreateMap<AddOrganisationAddressModel, CreateOrganisationAddressRequest>();
            CreateMap<OrganisationDetailsViewModel, AddOrganisationAddressModel>()
                .ForMember(dest => dest.OrganisationHashedId, opt => opt.MapFrom(src => src.HashedId))
                .ForMember(dest => dest.OrganisationName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.OrganisationReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber))
                .ForMember(dest => dest.OrganisationDateOfInception, opt => opt.MapFrom(src => src.DateOfInception))
                .ForMember(dest => dest.OrganisationType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.OrganisationStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PublicSectorDataSource, opt=> opt.MapFrom(src=> src.PublicSectorDataSource))
                .ForMember(dest => dest.ErrorDictionary, opt => opt.MapFrom(src => src.ErrorDictionary));
        }
    }
}