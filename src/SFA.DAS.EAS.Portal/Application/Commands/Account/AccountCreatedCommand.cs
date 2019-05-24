namespace SFA.DAS.EAS.Portal.Application.Commands.Account
{
    public class AccountCreatedCommand : ICommand
    {
        public long Id { get; private set; }
        public string Name { get; private set; }

        public AccountCreatedCommand(long id, string name = null)
        {
            Id = id;
            Name = name;
        }
    }
}
