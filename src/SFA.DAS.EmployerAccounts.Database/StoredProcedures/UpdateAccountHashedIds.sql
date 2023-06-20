CREATE PROCEDURE [employer_account].[UpdateAccountHashedIds]
	@accountId BIGINT,
	@hashedId VARCHAR(100),
	@publicHashedId VARCHAR(100)
AS
	UPDATE [employer_account].[Account]
	SET HashedId = @hashedId, PublicHashedId = @publicHashedId
	WHERE Id = @accountId