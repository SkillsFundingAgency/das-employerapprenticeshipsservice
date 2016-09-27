IF (@@servername NOT LIKE '%pp%' AND @@servername NOT LIKE '%prd%')
	BEGIN
	   RAISERROR('Server %s is in development - seeding test data',10,1,@@servername) WITH NOWAIT
		   SET IDENTITY_INSERT  [account].[User] ON
			IF (NOT EXISTS(SELECT * FROM [account].[User] WHERE Id = 1
				AND PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B'
				AND Email = 'floyd.price@test.local'))
			BEGIN 
				INSERT INTO [account].[User](Id, PireanKey, Email, FirstName, LastName) 
				VALUES(1,'758943A5-86AA-4579-86AF-FB3D4A05850B','floyd.price@test.local', 'Floyd', 'Price') 
			END 
			SET IDENTITY_INSERT  [account].[User] OFF
			
	END
ELSE
	BEGIN
		RAISERROR('Server %s is in production - finished',10,1,@@servername) WITH NOWAIT
	END

