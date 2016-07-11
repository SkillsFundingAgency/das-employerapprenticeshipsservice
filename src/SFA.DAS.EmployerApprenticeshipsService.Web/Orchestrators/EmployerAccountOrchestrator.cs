using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountOrchestrator
    {
        private readonly IMediator _mediator;

        public EmployerAccountOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
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
                DateOfIncorporation = response.DateOfIncorporation,
                RegisteredAddress = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}"
            };
        }
    }
}