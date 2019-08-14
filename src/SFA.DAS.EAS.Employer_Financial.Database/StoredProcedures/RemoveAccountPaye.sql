CREATE PROCEDURE [employer_financial].[RemoveAccountPaye]
	@accountId BIGINT,
	@empRef NVARCHAR(16)
AS
	DELETE FROM [employer_financial].[AccountPaye]
	WHERE AccountId = @accountId AND EmpRef = @empRef