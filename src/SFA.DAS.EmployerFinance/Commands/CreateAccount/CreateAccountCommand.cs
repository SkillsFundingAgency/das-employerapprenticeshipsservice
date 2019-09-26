using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.CreateAccount
{
    public class CreateAccountCommand : IAsyncRequest
    {
        public CreateAccountCommand(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }
        public string Name { get; }
    }
}