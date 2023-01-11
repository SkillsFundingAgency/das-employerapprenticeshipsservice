using AutoMapper;
using SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;

namespace SFA.DAS.EmployerAccounts.Web.Mappings;

public class OrganisationMappings : Profile
{
    public OrganisationMappings()
    {
        CreateMap<AddressViewModel, CreateOrganisationAddressRequest>()
            .ForMember(dest => dest.AddressThirdLine, opt => opt.Ignore());

        CreateMap<Address, AddressViewModel>()
            .ForMember(dest => dest.AddressFirstLine, opt => opt.MapFrom(src => src.Line1))
            .ForMember(dest => dest.AddressSecondLine, opt => opt.MapFrom(src => src.Line2))
            .ForMember(dest => dest.TownOrCity, opt => opt.MapFrom(src => src.TownOrCity))
            .ForMember(dest => dest.County, opt => opt.MapFrom(src => src.County))
            .ForMember(dest => dest.Postcode, opt => opt.MapFrom(src => src.PostCode))
            .ForMember(dest => dest.ErrorDictionary, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PublicHashedAccountId, opt => opt.Ignore());


        CreateMap<FindOrganisationAddressViewModel, AddOrganisationAddressViewModel>()
            .ForMember(dest => dest.Address, opt => opt.Ignore())
            .ForAllMembers(m => m.AllowNull());

        CreateMap<OrganisationDetailsViewModel, AddOrganisationAddressViewModel>()
            .ForMember(dest => dest.Address, opt => opt.Ignore())
            .ForMember(dest => dest.OrganisationAddress, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.OrganisationDateOfInception, opt => opt.MapFrom(src => src.DateOfInception))
            .ForMember(dest => dest.OrganisationHashedId, opt => opt.MapFrom(src => src.HashedId))
            .ForMember(dest => dest.OrganisationName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.OrganisationReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber))
            .ForMember(dest => dest.OrganisationStatus, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.OrganisationType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.PublicSectorDataSource, opt => opt.MapFrom(src => src.PublicSectorDataSource))
            .ForMember(dest => dest.Sector, opt => opt.MapFrom(src => src.Sector))
            .ForMember(dest => dest.Valid, opt => opt.MapFrom(src => src.Valid))
            .ForMember(dest => dest.ErrorDictionary, opt => opt.Ignore());

        CreateMap<OrganisationDetailsViewModel, FindOrganisationAddressViewModel>()
            .ForMember(dest => dest.OrganisationHashedId, opt => opt.MapFrom(src => src.HashedId))
            .ForMember(dest => dest.OrganisationName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.OrganisationReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber))
            .ForMember(dest => dest.OrganisationDateOfInception, opt => opt.MapFrom(src => src.DateOfInception))
            .ForMember(dest => dest.OrganisationType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.OrganisationStatus, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.PublicSectorDataSource, opt => opt.MapFrom(src => src.PublicSectorDataSource))
            .ForMember(dest => dest.ErrorDictionary, opt => opt.MapFrom(src => src.ErrorDictionary))
            .ForMember(dest => dest.Postcode, opt => opt.Ignore())
            .ForMember(dest => dest.PostcodeError, opt => opt.Ignore())
            .ForMember(dest => dest.OrganisationAddress, opt => opt.MapFrom(src => src.Address));
    }
}