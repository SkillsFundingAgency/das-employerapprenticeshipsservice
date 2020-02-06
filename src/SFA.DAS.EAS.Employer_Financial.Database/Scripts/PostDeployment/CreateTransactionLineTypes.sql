IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 1))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (1, N'Levy')
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 2))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (2, N'Levy Adjustment')
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 3))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (3, N'Payment')
END
IF (EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 3 AND [Description] = 'Expired Levy'))
BEGIN
	UPDATE [employer_financial].[TransactionLineTypes] SET [Description] = N'Payment' WHERE TransactionType = 3
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 4))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (4, N'Transfer')
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 5))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (5, N'Expired Levy')
END