DECLARE @accountId bigint = 0

DELETE employer_financial.LevyDeclaration WHERE AccountId = @accountId
DELETE employer_financial.LevyDeclarationTopup WHERE AccountId = @accountId
DELETE employer_financial.TransactionLine WHERE AccountId = @accountId AND TransactionType IN (1, 2)