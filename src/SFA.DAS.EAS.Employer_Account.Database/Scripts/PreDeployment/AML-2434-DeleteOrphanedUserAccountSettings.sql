IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'employer_account' AND TABLE_NAME = 'UserAccountSettings')
BEGIN
	DELETE s
	FROM [employer_account].[UserAccountSettings] s
	LEFT JOIN [employer_account].[Membership] m ON m.AccountId = s.AccountId AND m.UserId = s.UserId
	WHERE m.AccountId IS NULL
END