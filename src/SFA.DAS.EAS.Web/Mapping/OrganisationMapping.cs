using AutoMapper;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Mapping
{
    public class OrganisationMapping : Profile
    {
        public OrganisationMapping()
        {
            CreateMap<AddOrganisationAddressModel, CreateOrganisationAddressRequest>();
        }
    }
}