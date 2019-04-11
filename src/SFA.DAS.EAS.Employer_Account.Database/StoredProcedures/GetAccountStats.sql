CREATE PROCEDURE [employer_account].[GetAccountStats]
	@accountId BIGINT = 0	
AS
	SET NOCOUNT ON

	SELECT
		(SELECT @accountId) AS AccountId,
		(SELECT COUNT(1) FROM [employer_account].[AccountHistory] WHERE AccountId = @accountId AND RemovedDate IS NULL) AS PayeSchemeCount,
		(SELECT COUNT(1) FROM [employer_account].[AccountLegalEntity] WHERE AccountId = @accountId AND Deleted IS NULL) AS OrganisationCount,
		(SELECT COUNT(1) FROM [employer_account].[Membership] WHERE AccountId = @accountId) AS TeamMemberCount,
		(SELECT COUNT(1) FROM [employer_account].[Invitation] WHERE AccountId = @accountId AND Status = 1) AS TeamMembersInvited