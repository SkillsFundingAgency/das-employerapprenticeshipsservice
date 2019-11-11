CREATE PROCEDURE [employer_financial].[RemoveAccountLegalEntity]
	@id BIGINT
AS
	DELETE FROM [employer_financial].[AccountLegalEntity] WHERE [Id] = @id