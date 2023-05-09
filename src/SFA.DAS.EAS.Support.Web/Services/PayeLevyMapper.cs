using System.Globalization;
using HMRC.ESFA.Levy.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Services;

public class PayeLevyMapper : IPayeLevyMapper
{
    public PayeSchemeLevyDeclarationViewModel MapPayeLevyDeclaration(PayeLevySubmissionsResponse model)
    {
        var payeLevyDeclaration = new PayeSchemeLevyDeclarationViewModel();

        if (model == null)
        {
            return payeLevyDeclaration;
        }

        payeLevyDeclaration.PayeSchemeName = model.PayeScheme?.Name;
        payeLevyDeclaration.PayeSchemeRef = model.PayeScheme?.ObscuredPayeRef;
        payeLevyDeclaration.LevyDeclarations = model.LevySubmissions?.Declarations?.Select(o => MapLevyDeclarationViewModel(o)).ToList();
        payeLevyDeclaration.PayeSchemeFormatedAddedDate = model.PayeScheme?.AddedDate == DateTime.MinValue ?
                                      string.Empty :
                                      ConvertDateTimeToDdmmyyyyFormat(model.PayeScheme.AddedDate);
        return payeLevyDeclaration;
    }

    private static DeclarationViewModel MapLevyDeclarationViewModel(Declaration declaration)
    {

        var levy = new DeclarationViewModel
        {
            SubmissionDate = GetSubmissionDate(declaration.SubmissionTime),
            PayrollDate = GetPayrollDate(declaration.PayrollPeriod),
            LevySubmissionId = declaration.Id,
            LevyDeclarationDescription = GetLevyDeclarationDescription(declaration),
            YearToDateAmount = GetYearToDateAmount(declaration),
            SubmissionStatus = declaration.LevyDeclarationSubmissionStatus
        };

        return levy;
    }

    private static string GetSubmissionDate(DateTime submissionTime)
    {
        return submissionTime.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
    }

    private static string GetLevyDeclarationDescription(Declaration levyDeclaration)
    {
        if (levyDeclaration.DateCeased.HasValue && levyDeclaration.DateCeased.Value != DateTime.MinValue)
        {
            return $"Ceased {ConvertDateTimeToDdmmyyyyFormat(levyDeclaration.DateCeased)}";
        }

        if (levyDeclaration.NoPaymentForPeriod)
        {
            return "No payment";
        }

        if (levyDeclaration.InactiveFrom.HasValue && levyDeclaration.InactiveFrom.Value != DateTime.MinValue)
        {
            return $"Inactive {ConvertDateTimeToDdmmyyyyFormat(levyDeclaration.InactiveFrom)} "
                + $"to {ConvertDateTimeToDdmmyyyyFormat(levyDeclaration.InactiveTo)}";
        }

        return string.Empty;
    }

    private static string ConvertDateTimeToDdmmyyyyFormat(DateTime? dateTime)
    {
        return dateTime?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }

    private static string GetYearToDateAmount(Declaration levyDeclaration)
    {
        if (levyDeclaration.NoPaymentForPeriod ||
            (levyDeclaration.DateCeased.HasValue && levyDeclaration.DateCeased != DateTime.MinValue) ||
            (levyDeclaration.InactiveFrom.HasValue && levyDeclaration.InactiveFrom.Value != DateTime.MinValue))
        {
            return string.Empty;
        }
        return $"£{levyDeclaration.LevyDueYearToDate.ToString("#,##0.00")}";
    }

    private static string GetPayrollDate(PayrollPeriod payrollPeriod)
    {
        if (payrollPeriod == null)
        {
            return string.Empty;
        }

        var month = payrollPeriod.Month + 3;

        if (month > 12)
        {
            month -= 12;
        }

        var monthName = new DateTime(2010, month, 1).ToString("MMM", CultureInfo.InvariantCulture);

        return $"{payrollPeriod.Year} {payrollPeriod.Month} ({monthName})";
    }
}