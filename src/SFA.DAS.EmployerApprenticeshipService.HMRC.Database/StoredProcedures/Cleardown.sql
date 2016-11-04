CREATE PROCEDURE [levy].[Cleardown]
AS
	DELETE FROM [levy].[EnglishFraction]
	DELETE FROM [levy].[LevyDeclaration]
	DELETE FROM [levy].[TopUpPercentage]
	DELETE FROM [levy].[TransactionLine]
RETURN 0
