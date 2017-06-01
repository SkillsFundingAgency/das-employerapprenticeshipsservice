CREATE TABLE [employer_account].[UserLegalEntitySettings]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[UserId] BIGINT NOT NULL,
	[EmployerAgreementId] BIGINT NOT NULL,
	[ReceiveNotifications] BIT NOT NULL DEFAULT(1),
	CONSTRAINT [FK_UserLegalEntitySettings_UserId] FOREIGN KEY(UserId) REFERENCES [employer_account].[User] ([Id]),
	CONSTRAINT [FK_UserLegalEntitySettings_EmployerAgreementId] FOREIGN KEY(EmployerAgreementId) REFERENCES [employer_account].[EmployerAgreement] ([Id])
)
