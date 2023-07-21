using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;

public class GetEmployerAgreementPdfQueryHandler : IRequestHandler<GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>
{
    private readonly IValidator<GetEmployerAgreementPdfRequest> _validator;
    private readonly IPdfService _pdfService;
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public GetEmployerAgreementPdfQueryHandler(IValidator<GetEmployerAgreementPdfRequest> validator, IPdfService pdfService, Lazy<EmployerAccountsDbContext> db)
    {
        _validator = validator;
        _pdfService = pdfService;
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

        var templatePartialViewName = await _db.Value.Agreements
            .Where(x => x.Id == message.LegalAgreementId)
            .Select(x => x.Template.PartialViewName)
            .SingleAsync(cancellationToken);

        var file = await _pdfService.SubstituteValuesForPdf($"{templatePartialViewName}.pdf");


        return new GetEmployerAgreementPdfResponse {FileStream = file};
    }
}