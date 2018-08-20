IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'employer_account' AND TABLE_NAME = 'UserAccountSettings')
BEGIN
	DELETE
	FROM employer_account.UserAccountSettings
	WHERE Id IN (
		SELECT d.Id
		FROM
		(
			SELECT AccountId, UserId, COUNT(1) AS Count
			FROM employer_account.UserAccountSettings
			GROUP BY AccountId, UserId
			HAVING COUNT(1) > 1
		) g
		CROSS APPLY (
			SELECT TOP (g.Count - 1) Id
			FROM employer_account.UserAccountSettings
			WHERE AccountId = g.AccountId
			AND UserId = g.UserId
			ORDER BY Id DESC
		) d
	)
END