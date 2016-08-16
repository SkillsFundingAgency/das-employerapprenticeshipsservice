using System.Data;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationHandler : IAsyncRequestHandler<GetHmrcEmployerInformationQuery, GetHmrcEmployerInformationResponse>
    {
        private readonly IValidator<GetHmrcEmployerInformationQuery> _validator;
        private readonly IHmrcService _hmrcService;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public GetHmrcEmployerInformationHandler(IValidator<GetHmrcEmployerInformationQuery> validator, IHmrcService hmrcService, IMediator mediator, ILogger logger)
        {
            _validator = validator;
            _hmrcService = hmrcService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<GetHmrcEmployerInformationResponse> Handle(GetHmrcEmployerInformationQuery message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var empref = await _hmrcService.DiscoverEmpref(message.AuthToken);

            var emprefInformation = await _hmrcService.GetEmprefInformation(message.AuthToken, empref);

            var schemeCheck = await _mediator.SendAsync(new GetPayeSchemeInUseQuery { Empref = empref });

            if (schemeCheck.PayeScheme != null)
            {
                _logger.Warn($"PAYE scheme {empref} already in use.");
                throw new ConstraintException("PAYE scheme already in use");
            }


            return new GetHmrcEmployerInformationResponse { EmployerLevyInformation = emprefInformation, Empref = empref };
        }
    }
}
