namespace SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails;

public class UpdateOrganisationDetailsCommand : IAsyncRequest
{
    public long AccountLegalEntityId { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }

    public string HashedAccountId { get; set; }

    /// <summary>
    ///     The UserRef of the user.
    /// </summary>
    /// <remarks>
    ///     This is not the Id of the user. Not renaming it as UserId seems to be used 
    ///     comprehensively for UserRef.
    /// </remarks>
    public string UserId { get; set; }
}