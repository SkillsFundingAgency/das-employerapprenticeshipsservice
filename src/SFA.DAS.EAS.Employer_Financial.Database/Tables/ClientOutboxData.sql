CREATE TABLE [dbo].[ClientOutboxData]
(
	[MessageId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
	[EndpointName] NVARCHAR(150) NOT NULL,
	[Created] DATETIME NOT NULL,
	[Dispatched] BIT NOT NULL DEFAULT(0),
	[DispatchedAt] DATETIME NULL,
	[Operations] NVARCHAR(MAX) NOT NULL
)
GO

CREATE INDEX [IX_ClientOutboxData] ON [dbo].[ClientOutboxData] ([Created] ASC) WHERE [Dispatched] = 0
GO