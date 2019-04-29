namespace SFA.DAS.EmployerAccounts.Commands.CreateCohort
{
    public class CreateCohortCommand : ICommand
    {
        public long AccountId { get; set; }

        public string Reference { get; set; }

    }   
}