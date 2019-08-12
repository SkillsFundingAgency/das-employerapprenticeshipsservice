CREATE PROCEDURE [employer_financial].[CreateAccountPaye]
	@accountId BIGINT,
	@empRef NVARCHAR(16),
	@aorn VARCHAR(25) = NULL
AS
	INSERT INTO [employer_financial].[AccountPaye]
		(AccountId, EmpRef, Aorn)
	VALUES
		(@accountId, @empRef, @aorn)
