CREATE TABLE [dbo].[Invitation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[ExpiryDate] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
 [RoleId] INT NOT NULL, 
    CONSTRAINT [PK_Invitation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [FK_Invitation_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role]([Id]),
    CONSTRAINT [FK_Invitation_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account]([Id])
) ON [PRIMARY]

GO
