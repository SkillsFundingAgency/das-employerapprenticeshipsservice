IF (NOT EXISTS(SELECT * FROM [employer_financial].[TopUpPercentage] WHERE Id = 1 AND DateFrom = '2015-01-01 00:00:00.000'))
BEGIN 
	INSERT INTO [employer_financial].[TopUpPercentage]
	(DateFrom,Amount)
	VALUES
	('2015-01-01 00:00:00.000',0.1)
END