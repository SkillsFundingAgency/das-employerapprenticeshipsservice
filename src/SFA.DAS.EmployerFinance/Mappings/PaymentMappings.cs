using System;
using AutoMapper;
using SFA.DAS.EmployerFinance.Models.Payments;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class PaymentMappings : Profile
    {
        public PaymentMappings()
        {
            CreateMap<Payment, PaymentDetails>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest =>dest.PeriodEnd, opt => opt.Ignore())
                .ForMember(dest =>dest.ProviderName, opt => opt.Ignore())
                .ForMember(dest =>dest.CourseName, opt => opt.Ignore())
                .ForMember(dest =>dest.CourseLevel, opt => opt.Ignore())
                .ForMember(dest =>dest.CourseStartDate, opt => opt.Ignore())
                .ForMember(dest =>dest.HistoricProviderName, opt => opt.Ignore())
                .ForMember(dest =>dest.ApprenticeName, opt => opt.Ignore())
                .ForMember(dest =>dest.ApprenticeNINumber, opt => opt.Ignore())
                .ForMember(dest =>dest.PathwayName, opt => opt.Ignore());
        }
    }
}
