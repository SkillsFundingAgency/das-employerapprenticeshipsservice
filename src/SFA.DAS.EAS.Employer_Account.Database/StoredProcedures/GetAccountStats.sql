CREATE PROCEDURE [employer_account].[GetAccountStats]
	@accountId BIGINT = 0	
AS
	SELECT	MAX(acc.Id) as AccountId,
			COUNT(DISTINCT his.Id) as PayeSchemeCount, 
			COUNT(DISTINCT ag.Id) as OrganisationCount, 
			COUNT(DISTINCT mem.UserId) as TeamMemberCount 
	FROM [employer_account].[Account] acc
	INNER JOIN [employer_account].[AccountHistory] his ON acc.Id = his.AccountId
	INNER JOIN [employer_account].[EmployerAgreement] ag ON acc.Id = ag.AccountId
	INNER JOIN [employer_account].[Membership] mem ON acc.id = mem.AccountId
	WHERE acc.Id = @accountId
	GROUP BY acc.Id