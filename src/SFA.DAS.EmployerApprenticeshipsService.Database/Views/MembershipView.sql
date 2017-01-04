CREATE VIEW [account].[MembershipView]
AS
SELECT m.*, 
	CONVERT(varchar(64), u.PireanKey) AS UserRef, 
	u.Email,
	u.FirstName,
	u.LastName, 
	a.Name AS AccountName, 
	a.HashedId as HashedAccountId, 
	r.Name AS RoleName
FROM [account].[Membership] m
	JOIN [account].[User] u
		ON u.Id = m.UserId
	JOIN [account].[Account] a
		ON a.Id = m.AccountId
	JOIN [account].[Role] r
		ON r.Id = m.RoleId