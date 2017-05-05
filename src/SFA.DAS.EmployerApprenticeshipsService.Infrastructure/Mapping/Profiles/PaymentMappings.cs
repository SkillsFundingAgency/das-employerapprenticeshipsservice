using AutoMapper;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Infrastructure.Mapping.Profiles
{
    public class PaymentMappings : Profile
    {
        public PaymentMappings()
        {
            CreateMap<Payment, PaymentEntry>();
            CreateMap<Provider.Events.Api.Types.Payment, PaymentDetails>();
        }
    }
}
