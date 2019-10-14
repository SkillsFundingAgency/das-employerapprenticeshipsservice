CREATE PROCEDURE [employer_financial].[RenameAccount]
	@id BIGINT,
	@name NVARCHAR(100)
AS
	UPDATE [employer_financial].[Account]
		SET [Name] = @name
	WHERE Id = @id
