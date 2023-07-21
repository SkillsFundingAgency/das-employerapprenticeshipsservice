CREATE TABLE [employer_account].[UserAornFailedAttempts](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[AttemptTimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_UserAornFailedAttempt] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [employer_account].[UserAornFailedAttempts]  WITH CHECK ADD  CONSTRAINT [FK_UserAornFailedAttempt_User] FOREIGN KEY([UserId])
REFERENCES [employer_account].[User] ([Id])
GO

ALTER TABLE [employer_account].[UserAornFailedAttempts] CHECK CONSTRAINT [FK_UserAornFailedAttempt_User]
GO
CREATE NONCLUSTERED INDEX [IX_UserAornFailedAttempts_UserId] ON [employer_account].[UserAornFailedAttempts] ([UserId])