using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommand : IAsyncRequest
    {
        public string UserRef { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string CorrelationId { get; set; }
    }
}
