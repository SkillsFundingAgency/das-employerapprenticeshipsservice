CREATE PROCEDURE [employer_financial].[GetCurrentFractionForScheme]
	@accountId BIGINT,
	@empRef NVARCHAR(50)
AS
	IF EXISTS(SELECT TOP 1 * FROM [employer_financial].[EnglishFractionOverride] WHERE AccountId = @accountId AND EmpRef = @empRef AND DateFrom <= GETDATE())
	BEGIN
		SELECT TOP 1 Id, Amount, EmpRef, DateFrom AS DateCalculated FROM [employer_financial].[EnglishFractionOverride] WHERE AccountId = @accountId AND EmpRef = @empRef AND DateFrom <= GETDATE()
	END
	ELSE
	BEGIN
		SELECT top 1 * FROM [employer_financial].[EnglishFraction] WHERE EmpRef = @empRef Order by DateCalculated desc
	END
