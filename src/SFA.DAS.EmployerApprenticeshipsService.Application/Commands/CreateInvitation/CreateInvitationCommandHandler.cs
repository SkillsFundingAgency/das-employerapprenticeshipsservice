using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandHandler : AsyncRequestHandler<CreateInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IAccountTeamRepository _accountTeamRepository;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateInvitationCommand> _validator;

        public CreateInvitationCommandHandler(IInvitationRepository invitationRepository, IAccountTeamRepository accountTeamRepository, IMediator mediator)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (accountTeamRepository == null)
                throw new ArgumentNullException(nameof(accountTeamRepository));
            _invitationRepository = invitationRepository;
            _accountTeamRepository = accountTeamRepository;
            _mediator = mediator;
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

            await _mediator.SendAsync(new SendNotificationCommand
            {
                UserId =owner.UserId,
                Data = new EmailContent
                {
                    RecipientsAddress = message.Email,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Data = new Dictionary<string, string> { { "",""} }
                },
                DateTime = DateTime.UtcNow,
                MessageFormat = MessageFormat.Email,
                ForceFormat = true,
                TemplatedId = ""
            });
        }
    }
}