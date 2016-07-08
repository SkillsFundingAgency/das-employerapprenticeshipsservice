using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class HomeOrchestrator : IOrchestrator
    {
        private readonly IMediator _mediator;

        public HomeOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task GetUsers()
        {
            await _mediator.SendAsync(new GetUsersQuery());
        }
    }
}