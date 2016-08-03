using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationHandler : IAsyncRequestHandler<GetHmrcEmployerInformatioQuery, GetHmrcEmployerInformatioResponse>
    {
        private readonly IValidator<GetHmrcEmployerInformatioQuery> _validator;
        private readonly IHmrcService _hmrcService;

        public GetHmrcEmployerInformationHandler(IValidator<GetHmrcEmployerInformatioQuery> validator, IHmrcService hmrcService)
        {
            _validator = validator;
            _hmrcService = hmrcService;
        }

        public async Task<GetHmrcEmployerInformatioResponse> Handle(GetHmrcEmployerInformatioQuery message)
        {
            var result = _validator.Validate(message);
            
            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var emprefInformation = await _hmrcService.GetEmprefInformation(message.AuthToken, message.Empref);
            
            return new GetHmrcEmployerInformatioResponse {EmployerLevyInformation = emprefInformation};
        }
    }
}
