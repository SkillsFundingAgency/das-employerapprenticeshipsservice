IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 1))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (1, N'Levy')
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[TransactionLineTypes] WHERE TransactionType = 2))
BEGIN
	INSERT [employer_financial].[TransactionLineTypes] ([TransactionType], [Description]) VALUES (2, N'Levy Adjustment')
END