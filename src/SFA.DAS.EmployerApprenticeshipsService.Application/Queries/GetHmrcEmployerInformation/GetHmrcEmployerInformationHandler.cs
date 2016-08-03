using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationHandler : IAsyncRequestHandler<GetHmrcEmployerInformationQuery, GetHmrcEmployerInformationResponse>
    {
        private readonly IValidator<GetHmrcEmployerInformationQuery> _validator;
        private readonly IHmrcService _hmrcService;

        public GetHmrcEmployerInformationHandler(IValidator<GetHmrcEmployerInformationQuery> validator, IHmrcService hmrcService)
        {
            _validator = validator;
            _hmrcService = hmrcService;
        }

        public async Task<GetHmrcEmployerInformationResponse> Handle(GetHmrcEmployerInformationQuery message)
        {
            var result = _validator.Validate(message);
            
            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var emprefInformation = await _hmrcService.GetEmprefInformation(message.AuthToken, message.Empref);
            
            return new GetHmrcEmployerInformationResponse {EmployerLevyInformation = emprefInformation};
        }
    }
}
