﻿using AutoMapper;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EmployerAccounts.Events.Messages;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.EAS.Infrastructure.Mapping.Profiles
{
    public class PaymentMappings : Profile
    {
        public PaymentMappings()
        {
            CreateMap<Payment, PaymentDetails>();
            CreateMap<PaymentDetails, PaymentCreatedMessage>();
        }
    }
}
