/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


IF (NOT EXISTS(SELECT * FROM [employer_financial].[TopUpPercentage] WHERE Id = 1
	AND DateFrom = '2015-01-01 00:00:00.000'))
BEGIN 
	INSERT INTO [employer_financial].[TopUpPercentage]
	(DateFrom,Amount)
	VALUES
	('2015-01-01 00:00:00.000',0.1)
END 

IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 1))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (1, N'Levy')
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 2))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (2, N'Levy Adjustment')
END


IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[PaymentTransactionTypes] WHERE TransactionType = 1))
BEGIN
	INSERT [employer_financial].[PaymentTransactionTypes] ([TransactionType], [Description]) VALUES (N'1', N'Monthly Payment')
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[PaymentTransactionTypes] WHERE TransactionType = 2))
BEGIN
	INSERT [employer_financial].[PaymentTransactionTypes] ([TransactionType], [Description]) VALUES (N'2', N'Completion Payment')
END
