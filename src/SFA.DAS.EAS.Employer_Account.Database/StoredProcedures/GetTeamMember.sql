CREATE PROCEDURE [employer_account].[GetTeamMember]
	@hashedAccountId  NVARCHAR(50),
	@externalUserId NVARCHAR(50)
AS
	SELECT * FROM [employer_account].[MembershipView] m 
	INNER JOIN [employer_account].account a ON a.id=m.accountid 
	WHERE a.HashedId = @hashedAccountId AND UserRef = @externalUserId
