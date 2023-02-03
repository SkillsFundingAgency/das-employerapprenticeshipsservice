using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;

public class GetEmployerAgreementPdfQueryHandler : IRequestHandler<GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>
{
    private readonly IValidator<GetEmployerAgreementPdfRequest> _validator;
    private readonly IPdfService _pdfService;
    private readonly IHashingService _hashingService;
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public GetEmployerAgreementPdfQueryHandler(IValidator<GetEmployerAgreementPdfRequest> validator, IPdfService pdfService, IHashingService hashingService, Lazy<EmployerAccountsDbContext> db)
    {
        _validator = validator;
        _pdfService = pdfService;
        _hashingService = hashingService;
        _db = db;
    }

    public async Task<GetEmployerAgreementPdfResponse> Handle(GetEmployerAgreementPdfRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        if (validationResult.IsUnauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        var employerAgreementId = _hashingService.DecodeValue(message.HashedLegalAgreementId);

        var templatePartialViewName = await _db.Value.Agreements
            .Where(x => x.Id == employerAgreementId)
            .Select(x => x.Template.PartialViewName)
            .SingleAsync(cancellationToken);

        var file = await _pdfService.SubsituteValuesForPdf($"{templatePartialViewName}.pdf");


        return new GetEmployerAgreementPdfResponse {FileStream = file};
    }
}