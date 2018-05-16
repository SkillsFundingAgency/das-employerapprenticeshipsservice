IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[PaymentTransactionTypes] WHERE TransactionType = 1))
BEGIN
	INSERT [employer_financial].[PaymentTransactionTypes] ([TransactionType], [Description]) VALUES (N'1', N'Monthly Payment')
END
IF (NOT EXISTS(SELECT TransactionType FROM [employer_financial].[PaymentTransactionTypes] WHERE TransactionType = 2))
BEGIN
	INSERT [employer_financial].[PaymentTransactionTypes] ([TransactionType], [Description]) VALUES (N'2', N'Completion Payment')
END