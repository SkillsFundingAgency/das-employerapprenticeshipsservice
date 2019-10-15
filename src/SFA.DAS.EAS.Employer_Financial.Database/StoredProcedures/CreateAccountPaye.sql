CREATE PROCEDURE [employer_financial].[CreateAccountPaye]
	@accountId BIGINT,
	@empRef NVARCHAR(16),
	@name VARCHAR(500) NULL, 
	@aorn VARCHAR(25) = NULL
AS
	INSERT INTO [employer_financial].[AccountPaye]
		(AccountId, EmpRef, [Name], Aorn)
	VALUES
		(@accountId, @empRef, @name, @aorn)
