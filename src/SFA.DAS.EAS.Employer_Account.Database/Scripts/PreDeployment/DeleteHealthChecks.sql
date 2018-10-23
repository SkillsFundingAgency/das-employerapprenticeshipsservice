IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'employer_account' 
                 AND  TABLE_NAME = 'HealthChecks'))
BEGIN
DELETE FROM HealthChecks
END