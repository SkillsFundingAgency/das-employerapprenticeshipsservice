CREATE PROCEDURE [levy].[Cleardown]
AS
	DELETE FROM [levy].[EnglishFraction]
	DELETE FROM [levy].[LevyDeclaration]
	DELETE FROM [levy].[TopUpPercentage]
	DELETE FROM [levy].[TransactionLine]
	DELETE FROM [levy].[Payment]
	DELETE FROM [levy].[PeriodEnd]
	DELETE FROM [levy].[LevyDeclarationTopup]
	DELETE FROM [levy].[EnglishFractionCalculationDate]
RETURN 0
