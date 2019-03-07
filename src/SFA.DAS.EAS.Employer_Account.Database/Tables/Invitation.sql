CREATE TABLE [employer_account].[Invitation](
	[Id] BIGINT IDENTITY(1,1) NOT NULL,
	[AccountId] BIGINT NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[ExpiryDate] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
 [Role] TINYINT NOT NULL, 
    CONSTRAINT [PK_Invitation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [FK_Invitation_Account] FOREIGN KEY ([AccountId]) REFERENCES [employer_account].[Account]([Id])
) ON [PRIMARY]

GO
CREATE INDEX [IX_Invitation] ON [employer_account].[Invitation] ([Email], [Status], [ExpiryDate]) INCLUDE ([AccountId], [Name], [Role])

GO
CREATE INDEX [IX_Invitation_AccountId_Status] ON [employer_account].[Invitation]([AccountId], [Status]) INCLUDE ([Email], [ExpiryDate],	[Name],	[Role])