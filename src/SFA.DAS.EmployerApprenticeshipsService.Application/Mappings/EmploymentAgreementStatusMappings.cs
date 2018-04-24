using System.Linq;
using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
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
            CreateMap<GetAccountEmployerAgreementsQueryHandlerProjection, EmployerAgreementStatusDto>();

            CreateMap<EmployerAgreement, SignedEmployerAgreementDetailsDto>()
                .ForMember(ead => ead.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
                .ForMember(ead => ead.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
                .ForMember(ead => ead.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));

            CreateMap<LegalEntity, LegalEntityDto>();

            CreateMap<EmployerAgreement, PendingEmployerAgreementDetailsDto>().ForMember(ead => ead.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
                .ForMember(ead => ead.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
                .ForMember(ead => ead.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));

            CreateMap<IGrouping<LegalEntity, EmployerAgreement>, EmployerAgreementStatusDto>()
                .ForMember(d => d.LegalEntity, o => o.MapFrom(g => g.Key))
                .ForMember(d => d.Pending, o => o.MapFrom(g => g
                    .OrderByDescending(a => a.Template.VersionNumber)
                    .FirstOrDefault(a => a.StatusId == EmployerAgreementStatus.Pending)))
                .ForMember(d => d.Signed, o => o.MapFrom(g => g
                    .OrderByDescending(a => a.Template.VersionNumber)
                    .FirstOrDefault(a => a.StatusId == EmployerAgreementStatus.Signed)));
        }
    }
}