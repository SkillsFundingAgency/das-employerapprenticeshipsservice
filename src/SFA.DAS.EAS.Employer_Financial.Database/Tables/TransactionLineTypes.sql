CREATE TABLE [employer_financial].[TransactionLineTypes]
(	
    [TransactionType] TINYINT NOT NULL , 
    [Description] VARCHAR(100) NOT NULL, 
    CONSTRAINT [PK_TransactionLineTypes] PRIMARY KEY ([TransactionType]) 
)