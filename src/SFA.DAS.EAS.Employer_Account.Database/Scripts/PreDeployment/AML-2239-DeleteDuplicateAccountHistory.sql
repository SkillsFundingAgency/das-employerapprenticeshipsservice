-- Store the original data
IF(EXISTS (SELECT TOP(1) * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'employer_account'))
BEGIN
	IF (NOT EXISTS (SELECT * 
					 FROM INFORMATION_SCHEMA.TABLES 
					 WHERE TABLE_SCHEMA = 'employer_account' 
					 AND  TABLE_NAME = 'AccountHistoryNonUnique'))
	BEGIN

		CREATE TABLE [employer_account].[AccountHistoryNonUnique]
		(
			[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1,1),
			[AccountId] BIGINT NOT NULL,
			[PayeRef] VARCHAR(20) NOT NULL,
			[AddedDate] DATETIME NOT NULL,
			[RemovedDate] DATETIME NULL
		)


		SET IDENTITY_INSERT [employer_account].[AccountHistoryNonUnique] ON

		INSERT [employer_account].[AccountHistoryNonUnique]([Id], [AccountId], [PayeRef], [AddedDate], [RemovedDate])

		SELECT [Id], [AccountId], [PayeRef], [AddedDate], [RemovedDate] FROM [employer_account].[AccountHistory]

		SET IDENTITY_INSERT [employer_account].[AccountHistoryNonUnique] OFF

		-- Clean up the duplicates
		DELETE ah FROM [employer_account].[AccountHistory] ah
		INNER JOIN (
			SELECT h.AccountId, h.PayeRef, MIN(h.AddedDate) as minAddedDate
			FROM employer_account.AccountHistory h
			INNER JOIN (
				SELECT AccountId, PayeRef
				FROM employer_account.AccountHistory
				WHERE RemovedDate IS NULL
				GROUP BY AccountId, PayeRef
				HAVING COUNT(1) > 1
			) d ON d.AccountId = h.AccountId AND d.PayeRef = h.PayeRef
			GROUP BY h.AccountId, h.PayeRef
		) dupes
			ON dupes.AccountId = ah.[AccountId]
			AND dupes.[PayeRef] = ah.[PayeRef]
			AND dupes.minAddedDate < ah.AddedDate
	END
END