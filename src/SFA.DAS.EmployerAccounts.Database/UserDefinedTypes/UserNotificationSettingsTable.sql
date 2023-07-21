CREATE TYPE [employer_account].[UserNotificationSettingsTable] AS TABLE
(
	[AccountId] BIGINT NOT NULL,
	[ReceiveNotifications] BIT NOT NULL,
	PRIMARY KEY CLUSTERED ([AccountId] ASC) WITH (IGNORE_DUP_KEY = OFF)
)
