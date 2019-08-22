CREATE PROCEDURE [employer_financial].[CreateAccount]
	@id BIGINT,
	@name NVARCHAR(100)
AS
	INSERT INTO [employer_financial].[Account]
		(Id, [Name])
	VALUES
		(@id, @name)
