CREATE TABLE [employer_financial].[PaymentTransactionTypes]
(	
	[TransactionType] TINYINT NOT NULL ,
    [Description] VARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_PaymentTransactionTypes] PRIMARY KEY ([TransactionType])
)