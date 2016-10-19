using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResendInvitation
{
    public class ResendInvitationCommandHandler : AsyncRequestHandler<ResendInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;
        private readonly ResendInvitationCommandValidator _validator;

        public ResendInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator, EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (employerApprenticeshipsServiceConfiguration == null)
                throw new ArgumentNullException(nameof(employerApprenticeshipsServiceConfiguration));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
            _validator = new ResendInvitationCommandValidator();
        }

        protected override async Task HandleCore(ResendInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (owner == null || (Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var existing = await _invitationRepository.Get(owner.AccountId, message.Email);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation not found" } });

            if (existing.Status == InvitationStatus.Accepted)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Accepted invitations cannot be resent" } });

            existing.Status = InvitationStatus.Pending;
            var expiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8);
            existing.ExpiryDate = expiryDate;
            
            await _invitationRepository.Resend(existing);

            await _mediator.SendAsync(new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = message.Email,
                    TemplateId = "3edf7c6e-0f1d-4d4f-a092-f2f73cce1bf0",
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "x",
                    SystemId = "x",
                    Tokens = new Dictionary<string, string> {
                        { "account_name", owner.AccountName },
                        { "base_url", _employerApprenticeshipsServiceConfiguration.DashboardUrl },
                        { "expiry_date", expiryDate.ToString("dd MMM yyy")}
                    }
                }
            });
        }
    }
}