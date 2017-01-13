CREATE PROCEDURE [employer_transactions].[Cleardown]
AS
	DELETE FROM [employer_transactions].[EnglishFraction]
	DELETE FROM [employer_transactions].[LevyDeclaration]
	DELETE FROM [employer_transactions].[TopUpPercentage]
	DELETE FROM [employer_transactions].[TransactionLine]
	DELETE FROM [employer_transactions].[Payment]
	DELETE FROM [employer_transactions].[PeriodEnd]
	DELETE FROM [employer_transactions].[LevyDeclarationTopup]
	DELETE FROM [employer_transactions].[EnglishFractionCalculationDate]
RETURN 0
