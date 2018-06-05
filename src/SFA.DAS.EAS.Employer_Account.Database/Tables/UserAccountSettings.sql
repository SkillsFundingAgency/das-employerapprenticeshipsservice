CREATE TABLE [employer_account].[UserAccountSettings]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[UserId] BIGINT NOT NULL,
	[AccountId] BIGINT NOT NULL,
	[ReceiveNotifications] BIT NOT NULL DEFAULT(1),
	CONSTRAINT [FK_UserAccountSettings_UserId] FOREIGN KEY(UserId) REFERENCES [employer_account].[User] ([Id]),
	CONSTRAINT [FK_UserAccountSettings_AccountId] FOREIGN KEY(AccountId) REFERENCES [employer_account].[Account] ([Id])
)
GO

CREATE UNIQUE INDEX [IX_UserAccountSettings] ON [employer_account].[UserAccountSettings] ([UserId], [AccountId]) INCLUDE ([ReceiveNotifications])