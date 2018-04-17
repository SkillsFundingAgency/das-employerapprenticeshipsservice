CREATE PROCEDURE [employer_financial].[GetCurrentFractionForScheme]
	@AccountId BIGINT,
	@EmpRef NVARCHAR(50)
AS
	IF EXISTS(SELECT TOP 1 * FROM [employer_financial].[EnglishFractionOverride] WHERE AccountId = @AccountId AND EmpRef = @EmpRef AND DateFrom <= GETDATE())
	BEGIN
		SELECT TOP 1 Id, Amount, EmpRef, DateFrom AS DateCalculated FROM [employer_financial].[EnglishFractionOverride] WHERE AccountId = @AccountId AND EmpRef = @EmpRef AND DateFrom <= GETDATE()
	END
	ELSE
	BEGIN
		SELECT top 1 * FROM [employer_financial].[EnglishFraction] WHERE EmpRef = @EmpRef Order by DateCalculated desc
	END
