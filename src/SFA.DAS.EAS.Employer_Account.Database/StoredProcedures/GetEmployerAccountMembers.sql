CREATE PROCEDURE [employer_account].[GetEmployerAccountMembers]
	@hashedAccountId VARCHAR(MAX)	
AS
	SELECT 
		tm.*, m.ReceiveNotifications AS CanReceiveNotifications
	FROM 
		[employer_account].[GetTeamMembers] tm
	LEFT JOIN
		[employer_account].[Membership] m
	ON
		tm.Id = m.UserId
	AND
		tm.AccountId = m.AccountId
    WHERE 
		tm.HashedId = @hashedAccountId
GO