﻿CREATE TABLE [dbo].[OutboxMessages]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Sent] DATETIME NOT NULL,
	[Published] DATETIME NULL,
	[Data] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_OutboxMessages] ON [dbo].[OutboxMessages] ([Sent] ASC, [Published] ASC)
GO