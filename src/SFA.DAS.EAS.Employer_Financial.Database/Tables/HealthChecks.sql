CREATE TABLE [dbo].[HealthChecks]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[PublishedEvent] DATETIME NOT NULL,
	[ReceivedEvent] DATETIME NULL
)