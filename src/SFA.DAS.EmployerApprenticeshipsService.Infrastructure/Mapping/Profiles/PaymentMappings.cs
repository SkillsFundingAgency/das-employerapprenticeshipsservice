using AutoMapper;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Infrastructure.Mapping.Profiles
{
    public class PaymentMappings : Profile
    {
        public PaymentMappings()
        {
            CreateMap<Payment, PaymentEntry>();
        }
    }
}
