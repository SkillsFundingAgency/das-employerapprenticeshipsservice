CREATE TABLE [dbo].[OutboxData]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Sent] DATETIME NOT NULL,
	[Published] DATETIME NULL,
	[Data] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_OutboxData] ON [dbo].[OutboxData] ([Sent] ASC, [Published] ASC)
GO