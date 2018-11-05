SET NOCOUNT ON;

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ReceiveNotifications'
          AND Object_ID = Object_ID(N'[employer_account].[Membership]'))
BEGIN
	PRINT 'ReceiveNotifications does not exist in table [employer_account].[Membership] no settings migrated';
END
ELSE IF OBJECT_ID('[employer_account].[UserAccountSettings]') IS NULL
BEGIN
	PRINT 'UserAccountSettings does not exist no settings to migrate';
END
ELSE
BEGIN

	BEGIN TRY
		BEGIN TRAN;

		PRINT 'UPDATING Membership';

		UPDATE m
		SET m.[ReceiveNotifications] = uas.[ReceiveNotifications]
		FROM [employer_account].[Membership] m
		INNER JOIN [employer_account].[UserAccountSettings] uas
			ON m.AccountId = uas.AccountId
			AND m.UserId = uas.UserId

		PRINT 'DROPPING UserAccountSettings';

		DROP TABLE [employer_account].[UserAccountSettings]

		COMMIT TRAN;

		PRINT '<<OK: TranCount:' + convert(varchar(10), @@TranCount);

	END TRY
	BEGIN CATCH
		PRINT 'Migration of ReceiveNotifications failed';
		PRINT Error_message();

		IF @@TRANCOUNT > 0
		BEGIN
			PRINT 'Rolling back transaction';
			ROLLBACK TRAN;
		END;

		PRINT '<<Error: TranCount:' + convert(varchar(10), @@TranCount);

		THROW;
	END CATCH;
END

