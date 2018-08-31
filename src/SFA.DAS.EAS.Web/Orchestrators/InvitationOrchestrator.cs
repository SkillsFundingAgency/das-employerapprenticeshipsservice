﻿using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.AcceptInvitation;
using SFA.DAS.EAS.Application.Commands.CreateInvitation;
using SFA.DAS.EAS.Application.Queries.GetInvitation;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Application.Queries.GetUserInvitations;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class InvitationOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        protected InvitationOrchestrator()
        {
            
        }

        public InvitationOrchestrator(IMediator mediator, ILog logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
        }

        public virtual async Task<OrchestratorResponse<InvitationView>> GetInvitation(string id)
        {
            var response = await _mediator.SendAsync(new GetInvitationRequest
            {
                Id = id
            });

            var result = new OrchestratorResponse<InvitationView>
            {
                Data = response.Invitation
            };

            return result;
        }

        public virtual async Task AcceptInvitation(long invitationId, string externalUserId)
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
                    HashedAccountId = model.HashedAccountId,
                    ExternalUserId = externalUserId,
                    NameOfPersonBeingInvited = model.Name,
                    EmailOfPersonBeingInvited = model.Email,
                    RoleIdOfPersonBeingInvited = model.Role
                });
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex.Message);
            }
            
        }

        public async Task<OrchestratorResponse<UserInvitationsViewModel>> GetAllInvitationsForUser(string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetUserInvitationsRequest
                {
                    UserId = externalUserId
                });

                var getUserAccountsQueryResponse = await _mediator.SendAsync(new GetUserAccountsQuery
                {
                    UserRef = externalUserId
                });

                var result = new OrchestratorResponse<UserInvitationsViewModel>
                {
                    Data = new UserInvitationsViewModel
                    {
                        Invitations = response.Invitations,
                        ShowBreadCrumbs = getUserAccountsQueryResponse.Accounts.AccountList.Count != 0
                    }
                };

                return result;
            }
            catch (InvalidRequestException ex)
            {
                
                _logger.Info(ex.Message);
            }

            return new OrchestratorResponse<UserInvitationsViewModel>
            {
                Data = null
            };
        }
    }
}