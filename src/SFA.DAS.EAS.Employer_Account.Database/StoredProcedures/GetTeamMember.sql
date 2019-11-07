CREATE PROCEDURE [employer_account].[GetTeamMember]
	@hashedAccountId  NVARCHAR(50),
	@externalUserId NVARCHAR(50)
AS
	SELECT * FROM [employer_account].[MembershipView] m 
	WHERE m.HashedAccountId = @hashedAccountId AND UserRef = @externalUserId
