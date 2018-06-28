CREATE TABLE [employer_account].[OutboxMessages]
(
	[Id] NVARCHAR(200) NOT NULL PRIMARY KEY,
	[Created] DATETIME NOT NULL,
	[Dispatched] DATETIME NULL,
	[Data] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_OutboxMessages] ON [employer_account].[OutboxMessages] ([Created] ASC, [Dispatched] ASC) INCLUDE ([Id])
GO