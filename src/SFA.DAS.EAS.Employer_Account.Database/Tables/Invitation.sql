﻿CREATE TABLE [employer_account].[Invitation](
	[Id] BIGINT IDENTITY(1,1) NOT NULL,
	[AccountId] BIGINT NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[ExpiryDate] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
 [RoleId] TINYINT NOT NULL, 
    CONSTRAINT [PK_Invitation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [FK_Invitation_Account] FOREIGN KEY ([AccountId]) REFERENCES [employer_account].[Account]([Id])
) ON [PRIMARY]

GO
