using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.AcceptInvitation;
using SFA.DAS.EAS.Application.Commands.CreateInvitation;
using SFA.DAS.EAS.Application.Queries.GetInvitation;
using SFA.DAS.EAS.Application.Queries.GetUserInvitations;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.ViewModels;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class InvitationOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public InvitationOrchestrator(IMediator mediator, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<InvitationView> GetInvitation(string id)
        {
            var response = await _mediator.SendAsync(new GetInvitationRequest
            {
                Id = id
            });

            return response.Invitation;
        }

        public async Task AcceptInvitation(long invitationId, string externalUserId)
        {
            await _mediator.SendAsync(new AcceptInvitationCommand
            {
                Id = invitationId,
                ExternalUserId = externalUserId
            });
        }

        public async Task CreateInvitation(InviteTeamMemberViewModel model, string externalUserId)
        {
            try
            {
                await _mediator.SendAsync(new CreateInvitationCommand
                {
                    HashedId = model.AccountHashedId,
                    ExternalUserId = externalUserId,
                    Name = model.Name,
                    Email = model.Email,
                    RoleId = model.Role
                });
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex);
            }
            
        }

        public async Task<UserInvitationsViewModel> GetAllInvitationsForUser(string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetUserInvitationsRequest
                {
                    UserId = externalUserId
                });

                return new UserInvitationsViewModel {Invitations = response.Invitations };
            }
            catch (InvalidRequestException ex)
            {
                
                _logger.Info(ex);
            }

            return null;
        }
    }
}