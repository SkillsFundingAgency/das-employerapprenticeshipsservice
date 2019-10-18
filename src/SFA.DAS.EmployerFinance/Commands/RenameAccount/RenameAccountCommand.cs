using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.RenameAccount
{
    public class RenameAccountCommand : IAsyncRequest
    {
        public RenameAccountCommand(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }
        public string Name { get; }
    }
}