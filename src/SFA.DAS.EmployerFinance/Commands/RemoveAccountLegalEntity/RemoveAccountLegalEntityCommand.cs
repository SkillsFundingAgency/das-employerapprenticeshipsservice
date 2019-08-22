using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.RemoveAccountLegalEntity
{
    public class RemoveAccountLegalEntityCommand : IAsyncRequest
    {
        public RemoveAccountLegalEntityCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
