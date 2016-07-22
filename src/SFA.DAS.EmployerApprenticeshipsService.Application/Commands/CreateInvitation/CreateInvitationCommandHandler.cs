using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandHandler : AsyncRequestHandler<CreateInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IAccountTeamRepository _accountTeamRepository;
        private readonly IValidator<CreateInvitationCommand> _validator;

        public CreateInvitationCommandHandler(IInvitationRepository invitationRepository, IAccountTeamRepository accountTeamRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (accountTeamRepository == null)
                throw new ArgumentNullException(nameof(accountTeamRepository));
            _invitationRepository = invitationRepository;
            _accountTeamRepository = accountTeamRepository;
            _validator = new CreateInvitationCommandValidator();
        }

        protected override async Task HandleCore(CreateInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var existing = await _invitationRepository.Get(message.AccountId, message.Email);

            if (existing != null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation not found" } });

            var owner = await _accountTeamRepository.GetMembership(message.AccountId, message.ExternalUserId);

            if (owner == null || (Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "User is not an Owner for this Account" } });

            await _invitationRepository.Create(new Invitation
            {
                AccountId = message.AccountId,
                Email = message.Email,
                Name = message.Name,
                RoleId = message.RoleId,
                Status = InvitationStatus.Pending,
                ExpiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8)
            });
        }
    }
}