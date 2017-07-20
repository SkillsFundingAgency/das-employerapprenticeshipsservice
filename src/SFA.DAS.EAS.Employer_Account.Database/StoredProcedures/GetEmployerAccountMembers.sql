CREATE PROCEDURE [employer_account].[GetEmployerAccountMembers]
	@hashedAccountId VARCHAR(MAX)	
AS
	SELECT 
		tm.*, s.ReceiveNotifications AS CanReceiveNotifications
	FROM 
		[employer_account].[GetTeamMembers] tm
	LEFT JOIN
		[employer_account].[UserAccountSettings] s
	ON
		tm.Id = s.UserId
	AND
		tm.AccountId = s.AccountId
    WHERE 
		tm.HashedId = @hashedAccountId
GO