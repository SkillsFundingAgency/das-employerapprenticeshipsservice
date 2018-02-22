CREATE PROCEDURE [employer_account].[UpdateAccountPublicHashedIds]
(
	@accounts [employer_account].[AccountPublicHashedIdsTable] READONLY
)
AS
	UPDATE t
	SET PublicHashedId = f.PublicHashedId
	FROM [employer_account].[Account] t
	INNER JOIN @accounts f ON f.AccountId = t.Id