CREATE PROCEDURE [levy].[Cleardown]
	@INCLUDETOPUPTABLE TINYINT = 0
AS
	DELETE FROM [levy].[EnglishFraction]
	DELETE FROM [levy].[LevyDeclaration]
	
	IF @INCLUDETOPUPTABLE = 0
	BEGIN
		DELETE FROM [levy].[TopUpPercentage]
	END

	DELETE FROM [levy].[TransactionLine]
	DELETE FROM [levy].[Payment]
	DELETE FROM [levy].[PeriodEnd]
	DELETE FROM [levy].[PaymentMetaData]
	DELETE FROM [levy].[LevyDeclarationTopup]
	DELETE FROM [levy].[EnglishFractionCalculationDate]
RETURN 0
