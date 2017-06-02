CREATE TYPE [employer_account].[UserNotificationSettingsTable] AS TABLE
(
	[EmployerAgreementId] BIGINT NOT NULL,
	[ReceiveNotifications] BIT NOT NULL,
	PRIMARY KEY CLUSTERED ([EmployerAgreementId] ASC)WITH (IGNORE_DUP_KEY = OFF)
)
