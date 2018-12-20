using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity
{
    public class RemoveLegalEntityCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }

        /// <summary>
        ///     The UserRef of the user.
        /// </summary>
        /// <remarks>
        ///     This is not the Id of the user. Not renaming it as UserId seems to be used 
        ///     comprehensively for UserRef.
        /// </remarks>
        public string UserId { get; set; }
        public string HashedLegalAgreementId { get; set; }
    }
}
