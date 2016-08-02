using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandHandler : AsyncRequestHandler<CreateInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;
        private readonly IValidator<CreateInvitationCommand> _validator;

        public CreateInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator, EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
            _validator = new CreateInvitationCommandValidator();
        }

        protected override async Task HandleCore(CreateInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            //Verify the caller an owner of the account
            var caller = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (caller == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this Account" } });
            if ((Role)caller.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            //Verify the email is not used by an existing team member for the account
            var existingTeamMember = await _membershipRepository.Get(message.AccountId, message.Email);

            if (existingTeamMember != null && existingTeamMember.IsUser)
                throw new InvalidRequestException(new Dictionary<string, string> { { "ExistingMember", "Invitee is already a Member of this team" } });

            //Verify the email is not used by an existing invitation for the account
            var existingInvitation = await _invitationRepository.Get(message.AccountId, message.Email);

            if (existingInvitation != null && existingInvitation.Status != InvitationStatus.Deleted)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "There is already an Invitation for this email" } });

            if (existingInvitation == null)
            {
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
            else
            {
                existingInvitation.Name = message.Name;
                existingInvitation.RoleId = message.RoleId;
                existingInvitation.Status = InvitationStatus.Pending;
                existingInvitation.ExpiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8);

                await _invitationRepository.Resend(existingInvitation);
            }

            await _mediator.SendAsync(new SendNotificationCommand
            {
                UserId = caller.UserId,
                Data = new EmailContent
                {
                    RecipientsAddress = message.Email,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Data = new Dictionary<string, string> { { "InviteeName",message.Name}, {"ReturnUrl", _employerApprenticeshipsServiceConfiguration.DashboardUrl } }
                },
                DateTime = DateTime.UtcNow,
                MessageFormat = MessageFormat.Email,
                ForceFormat = true,
                TemplatedId = ""
            });
        }
    }
}