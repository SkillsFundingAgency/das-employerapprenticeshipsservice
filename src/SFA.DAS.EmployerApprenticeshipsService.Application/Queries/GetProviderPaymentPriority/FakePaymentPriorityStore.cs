using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority
{
    public static class FakePaymentPriorityStore
    {
        private static List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI> Data { get; set; }

        public static List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI> GetData()
        {
            if (Data == null || !Data.Any())
            {
                Data = FakeData();  
            }
            return Data;
        }

        public static void UpdateData(List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI> data)
        {
            Data = data;
        }

        public static List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI> FakeData()
        {
            return new List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI>
                       {
                           new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                               {
                                   ProviderName =
                                       "Abba AB",
                                   ProviderId = 1111,
                                   PaymentPriority = 1
                               },
                           new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                               {
                                   ProviderName =
                                       "M&S Warps Ltd",
                                   ProviderId = 2222,
                                   PaymentPriority = 2
                               },
                           new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                               {
                                   ProviderName =
                                       "Valtech LIMITED",
                                   ProviderId = 3333,
                                   PaymentPriority = 3
                               }
                       };
        }

    }
}