using AutoMapper;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class AgreementMappings : Profile
    {
        public AgreementMappings()
        {
            CreateMap<EmployerAgreement, EmployerAgreementView>();
            CreateMap<GetEmployerAgreementResponse, EmployerAgreementViewModel>();
        }
    }
}