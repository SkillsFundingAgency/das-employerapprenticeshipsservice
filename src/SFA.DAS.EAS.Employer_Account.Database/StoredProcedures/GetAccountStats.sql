CREATE PROCEDURE [employer_account].[GetAccountStats]
	@accountId BIGINT = 0	
AS
	SELECT	acc.Id as AccountId,
			Count(distinct his.Id) as PAYECount, 
			Count(distinct ag.Id) as OrganisationCount, 
			Count(distinct mem.AccountId) as TeamMemberCount 
	FROM [employer_account].[Account] acc
	INNER JOIN [employer_account].[AccountHistory] his ON acc.Id = his.AccountId
	INNER JOIN [employer_account].[EmployerAgreement] ag ON acc.Id = ag.AccountId
	INNER JOIN [employer_account].[Membership] mem ON acc.id = mem.AccountId
	WHERE acc.Id = @accountId

