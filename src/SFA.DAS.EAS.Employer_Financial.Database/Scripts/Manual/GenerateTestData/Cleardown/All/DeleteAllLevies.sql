-- danger: here be dragons!
-- deletes *all* EnglishFractions, LevyDeclarations, LevyDeclarationTopups, and TransactionLines of type declaration and topup

delete employer_financial.EnglishFraction
delete employer_financial.LevyDeclaration
delete employer_financial.LevyDeclarationTopup
delete employer_financial.TransactionLine where TransactionType in (/*Declaration*/ 1, /*TopUp*/ 2)