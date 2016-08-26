CREATE PROCEDURE [levy].[Cleardown]
AS
	DELETE FROM [levy].[EnglishFraction]
	DELETE FROM [levy].[LevyDeclaration]
	DELETE FROM [levy].[TopUpPercentage]
RETURN 0
