using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferRequestsViewModel : IAuthorizationContextModel
    {
        public long AccountId { get; set; }

        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }
    }
}