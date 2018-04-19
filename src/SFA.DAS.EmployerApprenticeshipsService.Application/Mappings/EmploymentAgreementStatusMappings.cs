using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class EmploymentAgreementStatusMappings : Profile
    {
        public EmploymentAgreementStatusMappings()
        {
            CreateMap<GetAccountEmployerAgreementsQueryHandlerProjection, EmployerAgreementStatusView>();

            CreateMap<EmployerAgreement, SignedEmployerAgreementDetails>()
                .ForMember(ead => ead.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
                .ForMember(ead => ead.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
                .ForMember(ead => ead.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));
                

            CreateMap<EmployerAgreement, PendingEmployerAgreementDetails>().ForMember(ead => ead.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
                .ForMember(ead => ead.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
                .ForMember(ead => ead.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));
        }
    }
}