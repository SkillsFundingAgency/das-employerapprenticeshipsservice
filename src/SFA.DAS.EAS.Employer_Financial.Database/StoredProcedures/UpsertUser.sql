CREATE PROCEDURE [employer_financial].[UpsertUser]
	@userRef uniqueidentifier,
	@email nvarchar(255),
	@firstName nvarchar(max),
	@lastName nvarchar(max),
	@correlationId nvarchar(255)
AS
	MERGE [employer_financial].[User] AS [Target]
	USING (SELECT @userRef AS UserRef) AS [Source] 
	ON [Target].UserRef = [Source].UserRef
	WHEN MATCHED THEN  UPDATE SET [Target].Email = @email, [Target].FirstName = @firstName, [Target].LastName = @lastName, [Target].CorrelationId = COALESCE(@correlationId, [Target].CorrelationId)
	WHEN NOT MATCHED THEN  INSERT (UserRef, Email, FirstName, LastName, CorrelationId) VALUES (@userRef, @email, @firstName, @lastName, @correlationId);
