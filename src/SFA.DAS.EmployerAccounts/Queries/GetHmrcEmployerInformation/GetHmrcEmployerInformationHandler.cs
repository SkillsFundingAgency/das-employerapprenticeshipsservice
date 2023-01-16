﻿using System.Data;
using System.Threading;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.Hmrc;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;

public class GetHmrcEmployerInformationHandler : IRequestHandler<GetHmrcEmployerInformationQuery, GetHmrcEmployerInformationResponse>
{
    private readonly IValidator<GetHmrcEmployerInformationQuery> _validator;
    private readonly IHmrcService _hmrcService;
    private readonly IMediator _mediator;
    private readonly ILog _logger;


    public GetHmrcEmployerInformationHandler(IValidator<GetHmrcEmployerInformationQuery> validator, IHmrcService hmrcService, IMediator mediator, ILog logger)
    {
        _validator = validator;
        _hmrcService = hmrcService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<GetHmrcEmployerInformationResponse> Handle(GetHmrcEmployerInformationQuery message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var empref = await _hmrcService.DiscoverEmpref(message.AuthToken);

        if (string.IsNullOrEmpty(empref))
        {
            throw new NotFoundException($"{empref} no paye scheme exists");
        }

        var emprefInformation = await _hmrcService.GetEmprefInformation(message.AuthToken, empref);

        var schemeCheck = await _mediator.Send(new GetPayeSchemeInUseQuery { Empref = empref }, cancellationToken);

        if (schemeCheck.PayeScheme != null)
        {
            _logger.Warn($"PAYE scheme {empref} already in use.");
            throw new ConstraintException("PAYE scheme already in use");
        }
            
        return new GetHmrcEmployerInformationResponse { EmployerLevyInformation = emprefInformation, Empref = empref };
    }
}