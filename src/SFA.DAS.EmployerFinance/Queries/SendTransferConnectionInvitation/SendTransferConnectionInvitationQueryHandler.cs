using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.MarkerInterfaces;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<SendTransferConnectionInvitationQuery, SendTransferConnectionInvitationResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IMapper _mapper;

        public SendTransferConnectionInvitationQueryHandler(
            IEmployerAccountRepository employerAccountRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IMapper mapper)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _mapper = mapper;
        }

        public async Task<SendTransferConnectionInvitationResponse> Handle(SendTransferConnectionInvitationQuery message)
        {
            var receiverAccount = await _employerAccountRepository.Get(message.ReceiverAccountPublicHashedId);

            if (receiverAccount == null || receiverAccount.Id == message.AccountId)
            {
                ThrowValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You must enter a valid account ID");
            }

            var anyTransferConnectionToSameReceiverInvitations = 
                await _transferConnectionInvitationRepository.AnyTransferConnectionInvitations(
                    message.AccountId, 
                    receiverAccount.Id, 
                    new List<TransferConnectionInvitationStatus> { TransferConnectionInvitationStatus.Pending, TransferConnectionInvitationStatus.Approved});
               
            if (anyTransferConnectionToSameReceiverInvitations)
            {
                ThrowValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You can't connect with this employer because they already have a pending or accepted connection request");
            }

            var senderAccount = await _employerAccountRepository.Get(message.AccountId);

            return new SendTransferConnectionInvitationResponse
            {
                ReceiverAccount = _mapper.Map<AccountDto>(receiverAccount),
                SenderAccount = _mapper.Map<AccountDto>(senderAccount),
            };
        }

        private void ThrowValidationException<T>(Expression<Func<T, object>> property, string message) where T : class
        {
            var ex = new ValidationException();
            ex.AddError<T>(property, message);
            throw ex;
        }
    }
}