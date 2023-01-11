using AutoMapper;
using SFA.DAS.ReferenceData.Types.DTO;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class ReferenceDataMappings : Profile
{
    public ReferenceDataMappings()
    {
        CreateMap<Models.Organisation.Address, Address>();
        CreateMap<Models.PensionRegulator.Organisation, Organisation>()
            .ForMember(s => s.Code, opts => opts.Ignore())
            .ForMember(s => s.RegistrationDate, opts => opts.Ignore())
            .ForMember(s => s.SubType, opts => opts.Ignore())
            .ForMember(s => s.OrganisationStatus, opts => opts.Ignore())
            .ForMember(s => s.Sector, opts => opts.Ignore());
    }
}