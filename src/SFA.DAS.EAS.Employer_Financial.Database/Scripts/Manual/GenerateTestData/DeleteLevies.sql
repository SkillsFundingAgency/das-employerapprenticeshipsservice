--TODO DELETE BY ACCOUNTID
delete employer_financial.EnglishFraction
delete employer_financial.LevyDeclaration
delete employer_financial.LevyDeclarationTopup
delete employer_financial.TransactionLine where TransactionType in (1, 2)