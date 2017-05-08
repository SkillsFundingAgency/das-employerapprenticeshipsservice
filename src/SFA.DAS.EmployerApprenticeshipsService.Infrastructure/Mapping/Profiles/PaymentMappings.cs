using AutoMapper;
using SFA.DAS.EAS.Domain.Models.Payments;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.EAS.Infrastructure.Mapping.Profiles
{
    public class PaymentMappings : Profile
    {
        public PaymentMappings()
        {
            CreateMap<Payment, PaymentEntry>()
                .ForMember(target => target.CollectionPeriodId, opt => opt.MapFrom(src => src.CollectionPeriod.Id))
                .ForMember(target => target.CollectionPeriodMonth, opt => opt.MapFrom(src => src.CollectionPeriod.Month))
                .ForMember(target => target.CollectionPeriodYear, opt => opt.MapFrom(src => src.CollectionPeriod.Year))
                .ForMember(target => target.DeliveryPeriodMonth, opt => opt.MapFrom(src => src.DeliveryPeriod.Month))
                .ForMember(target => target.DeliveryPeriodYear, opt => opt.MapFrom(src => src.DeliveryPeriod.Year));

            CreateMap<Payment, PaymentDetails>();
        }
    }
}
