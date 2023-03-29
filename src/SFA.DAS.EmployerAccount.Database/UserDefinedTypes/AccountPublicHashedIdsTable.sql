CREATE TYPE [employer_account].[AccountPublicHashedIdsTable] AS TABLE
(
	[AccountId] BIGINT NOT NULL,
	[PublicHashedId] NVARCHAR(100) NOT NULL,
	PRIMARY KEY ([AccountId] ASC)
)