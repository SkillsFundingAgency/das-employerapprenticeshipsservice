namespace SFA.DAS.EmployerAccounts.Validation;

public class ValidationResult
{
    public bool IsUnauthorized { get; set; }
    public Dictionary<string, string> ValidationDictionary { get; set; }

    public ValidationResult()
    {
        ValidationDictionary = new Dictionary<string, string>();
    }

    public void AddError(string propertyName)
    {
        ValidationDictionary.Add(propertyName, $"{propertyName} has not been supplied");
    }

    public void AddError(string propertyName, string validationError)
    {
        ValidationDictionary.Add(propertyName, validationError);
    }

    public List<string> ErrorList => ValidationDictionary.Select(c => c.Key + "|" + c.Value).ToList();
    public bool FailedRuleValidation { get; set; }
    public bool FailedAuthorisationValidation { get; set; }
    public bool FailedGlobalRuleValidation { get; set; }
    public bool FailedTransferReceiverCheck { get; set; }
    public bool FailedAutoReservationCheck { get; set; }
    public bool FailedAgreementSignedCheck { get ; set ; }

    public bool IsValid()
    {
        if (ValidationDictionary == null)
        {
            return false;
        }

        return !ValidationDictionary.Any();
    }
}