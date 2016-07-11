using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class HomeOrchestrator : IOrchestrator
    {
        private readonly IMediator _mediator;

        //Required for running tests
        public HomeOrchestrator()
        {
            
        }

        public HomeOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual async Task<SignInUserViewModel> GetUsers()
        {
            var actual = await _mediator.SendAsync(new GetUsersQuery());

            return new SignInUserViewModel
            {
                AvailableUsers = actual.Users.Select(x =>
                                                new SignInUserModel
                                                {
                                                    Email = x.Email,
                                                    FirstName = x.FirstName,
                                                    LastName = x.LastName,
                                                    UserId = x.UserId
                                                }).ToList()
            };
        }

        public async Task<SelectEmployerViewModel> GetCompanyDetails(SelectEmployerModel model)
        {
            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = model.EmployerRef
            });

            if (response == null)
                return new SelectEmployerViewModel();

            return new SelectEmployerViewModel
            {
                CompanyNumber = response.CompanyNumber,
                CompanyName = response.CompanyName,
                DateOfIncorporation = response.DateOfIncorporation
            };
        }
    }
}