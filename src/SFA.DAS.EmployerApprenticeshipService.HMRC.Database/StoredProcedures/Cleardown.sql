CREATE PROCEDURE [levy].[Cleardown]
AS
	DELETE FROM [levy].[EnglishFraction]
	DELETE FROM [levy].[LevyDeclaration]
RETURN 0
