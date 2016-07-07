CREATE PROCEDURE [dbo].[Cleardown]
AS
	DELETE FROM [dbo].[Membership];
	DELETE FROM [dbo].[User];
	DBCC CHECKIDENT("[dbo].[User]", RESEED, 0)
	DELETE FROM [dbo].[Account];
	DBCC CHECKIDENT("[dbo].[Account]", RESEED, 0)
RETURN 0
