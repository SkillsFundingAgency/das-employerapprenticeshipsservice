using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferRequests;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class TransferRequestMappings : Profile
    {
        public TransferRequestMappings()
        {
            CreateMap<TransferRequest, TransferRequestDto>();
        }
    }
}