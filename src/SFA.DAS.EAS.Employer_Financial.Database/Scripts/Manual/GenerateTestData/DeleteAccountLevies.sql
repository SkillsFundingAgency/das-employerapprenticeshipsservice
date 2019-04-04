declare @accountId bigint = 0

delete employer_financial.LevyDeclaration where AccountId = @accountId
delete employer_financial.LevyDeclarationTopup where AccountId = @accountId
delete employer_financial.TransactionLine where AccountId = @accountId and TransactionType in (1, 2)