-- danger: here be dragons!
-- deletes *all* EnglishFractions, LevyDeclarations, LevyDeclarationTopups, and TransactionLines of type declaration and topup

DELETE employer_financial.EnglishFraction
DELETE employer_financial.LevyDeclaration
DELETE employer_financial.LevyDeclarationTopup
DELETE employer_financial.TransactionLine WHERE TransactionType IN (/*Declaration*/ 1, /*TopUp*/ 2)