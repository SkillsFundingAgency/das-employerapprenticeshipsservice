//using System.ComponentModel.DataAnnotations;
//using AutoMapper;
//using MediatR;
//using SFA.DAS.Authorization.ModelBinding;

//namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowance
//{
//    public class GetTransferAllowanceQuery : IAuthorizationContextModel, IAsyncRequest<GetTransferAllowanceResponse>
//    {
//        [IgnoreMap]
//        [Required]
//        public long? AccountId { get; set; }
//    }
//}