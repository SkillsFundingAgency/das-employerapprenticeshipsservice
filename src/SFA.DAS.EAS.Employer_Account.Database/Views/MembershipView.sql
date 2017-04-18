CREATE VIEW [employer_account].[MembershipView]
AS
SELECT m.*, 
	CONVERT(varchar(64), u.UserRef) AS UserRef, 
	u.Email,
	u.FirstName,
	u.LastName, 
	a.Name AS AccountName, 
	a.HashedId as HashedAccountId
FROM [employer_account].[Membership] m
	JOIN [employer_account].[User] u
		ON u.Id = m.UserId
	JOIN [employer_account].[Account] a
		ON a.Id = m.AccountId