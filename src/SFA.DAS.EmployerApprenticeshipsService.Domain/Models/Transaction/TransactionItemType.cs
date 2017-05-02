namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public enum TransactionItemType
    {
        Unknown = 0,
        Declaration = 1,
        TopUp = 2,
        Payment = 3,
        SFACoInvestment = 4,
        EmployerCoInvestment = 5,
        //This type is used to group SFA and Employer transactions together
        CoInvestment = 6,

    }
}