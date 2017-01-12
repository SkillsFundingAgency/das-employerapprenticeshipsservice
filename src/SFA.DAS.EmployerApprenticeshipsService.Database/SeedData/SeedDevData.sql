/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


EXECUTE account.Cleardown;

-- Role seed data
SET IDENTITY_INSERT  [account].[Role] ON 
IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 1
    AND Name = 'Owner'))
BEGIN 
    INSERT INTO [account].[Role](Id, Name) 
    VALUES(1, 'Owner') 
END 
ELSE 
BEGIN 
    UPDATE [account].[Role] 
    SET Name = 'Owner'
    WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 2
    AND Name = 'Transactor'))
BEGIN 
    INSERT INTO [account].[Role](Id, Name) 
    VALUES(2, 'Transactor') 
END 
ELSE 
BEGIN 
    UPDATE [account].[Role] 
    SET Name = 'Transactor'
    WHERE Id = 2
END 
IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 3
    AND Name = 'Viewer'))
BEGIN 
    INSERT INTO [account].[Role](Id, Name) 
    VALUES(3, 'Viewer') 
END 
ELSE 
BEGIN 
    UPDATE [account].[Role] 
    SET Name = 'Viewer'
    WHERE Id = 3
END 
SET IDENTITY_INSERT  [account].[Role] OFF


-- Account seed data
SET IDENTITY_INSERT  [account].[Account] ON
IF (NOT EXISTS(SELECT * FROM [account].[Account] WHERE Id = 1))
BEGIN 
    INSERT INTO [account].[Account](Id, Name, HashedId, CreatedDate) 
    VALUES(1, 'ACME LTD', 'KAKAKAKA', GETDATE()) 
END 
ELSE 
BEGIN 
    UPDATE [account].[Account] 
    SET Name = 'ACME LTD',
	HashedId = 'KAKAKAKA'
    WHERE Id = 1
END 
SET IDENTITY_INSERT  [account].[Account] OFF


-- User seed data 
SET IDENTITY_INSERT  [account].[User] ON
IF (NOT EXISTS(SELECT * FROM [account].[User] WHERE Id = 1
	AND PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B'
    AND Email = 'floyd.price@test.local'))
BEGIN 
    INSERT INTO [account].[User](Id, PireanKey, Email, FirstName, LastName) 
    VALUES(1,'758943A5-86AA-4579-86AF-FB3D4A05850B','floyd.price@test.local', 'Floyd', 'Price') 
END 
ELSE 
BEGIN 
    UPDATE [account].[User] 
    SET PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B', Email = 'floyd.price@test.local', FirstName = 'Floyd', LastName = 'Price'
    WHERE Id = 1
END 


SET IDENTITY_INSERT  [account].[User] OFF


-- Membership seed data
IF (NOT EXISTS(SELECT * FROM [account].[Membership] WHERE RoleId = 1
	AND [UserId] = 1
    AND AccountId = 1))
BEGIN 
    INSERT INTO [account].[Membership](RoleId, UserId, AccountId) 
    VALUES(1,1,1) 
END 
ELSE 
BEGIN 
    UPDATE [account].[Membership] 
    SET [UserId] = 1, AccountId = 1
    WHERE RoleId = 1
END 

-- EmployerAgreement Status
SET IDENTITY_INSERT  [account].[EmployerAgreementStatus] ON 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 1
	AND Name = 'Pending'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(1, 'Pending') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Pending'
	WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 2
	AND Name = 'Signed'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(2, 'Signed') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Signed'
	WHERE Id = 2
END 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 3
	AND Name = 'Expired'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(3, 'Expired') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Expired'
	WHERE Id = 3
END 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 4
	AND Name = 'Superceded'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(4, 'Superceded') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Superceded'
	WHERE Id = 4
END 
SET IDENTITY_INSERT  [account].[EmployerAgreementStatus] OFF

-- EmployerAgreement Template
SET IDENTITY_INSERT  [account].[EmployerAgreementTemplate] ON 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementTemplate] WHERE Id = 1
	AND [Ref] = 'SFA Employer Agreement V1.0BETA'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementTemplate](Id, [Text], CreatedDate, Ref, ReleasedDate) 
	VALUES(1, '<p>
1 Introduction
</p>

<p>
1.1 This Agreement is made between the Secretary of State for Education
through the Skills Funding Agency, an executive agency of the Department for
Education (the SFA) and [Name of employer], (the Employer).
</p>

<p>
1.2 The Agreement applies to any organisation that employs Apprentices and
sets out the terms and conditions on which the SFA will make payments to
Providers who have a Contract with the Employer to deliver Apprenticeships.
</p>

<p>
2 Definitions
</p>

<p>
“Agreement” means this agreement between the Employer and the SFA
</p>

<p>
“Apprentice” means those who receive apprenticeship training and (where 
required) end-point assessment through an apprenticeship framework or
standard funded by the Skills Funding Agency.
</p>

<p>
“Apprenticeship” means a job with an accompanying skills development
programme. This includes the training and (where required) end-point-
assessment for an employee as part of a job with an accompanying skills
development programme.
</p>

<p>
“Apprenticeship Framework” means [AGREE DEFINITION}
</p>

<p>
“Apprenticeship Levy” means a levy on UK employers to fund new
apprenticeships. In England, control of apprenticeship funding will be put in
the hands of employers through the Digital Apprenticeship Service
</p>

<p>
“Apprenticeship Standard” means [AGREE DEFINITION}
</p>

<p>
“Commitment Statement” means the details of Apprenticeship Standards or
Apprenticeship Frameworks which the Provider will deliver and the price
agreed for that delivery that is recorded on DAS
</p>

<p>
“Contract” means the Contract between the Employer and the Provider for the
delivery of Apprenticeships.
</p>

<p>
“Digital apprenticeship service” means the digital interface to services
designed to support the uptake of apprenticeships. The service is aimed
primarily at employers, with information coming from a range of different
sources including learning providers and apprenticeship assessment
organisations.
</p>

<p>
“Funding” means
</p>

<p>
“Funding Rules” means the document which sets out the detailed
requirements for Apprenticeships which the Employer and Provider must
comply with in order for the Provider to be funded by the SFA.
</p>

<p>
“Provider” means an organisation body who has a Contract with the Employer
to deliver Apprenticeships.
</p>

<p>
“Provider Agreement” means the agreement between the Provider and the
SFA setting out the condition on which the Provider will receive Funding from
the SFA.
</p>

<p>
“Register” means the Register of Apprenticeship Training Providers on which
all Providers must appear and the Register of Apprenticeship Assessment
Organisations on which all Assessment Organisations must appear.
</p>

<p>
3 Term of the Agreement
</p>

<p>
3.1 The Agreement will commence on XXXXX and shall end on XXXXX 
</p>


<p>
1 Introduction
</p>

<p>
1.1 This Agreement is made between the Secretary of State for Education
through the Skills Funding Agency, an executive agency of the Department for
Education (the SFA) and [Name of employer], (the Employer).
</p>

<p>
1.2 The Agreement applies to any organisation that employs Apprentices and
sets out the terms and conditions on which the SFA will make payments to
Providers who have a Contract with the Employer to deliver Apprenticeships.
</p>

<p>
2 Definitions
</p>

<p>
“Agreement” means this agreement between the Employer and the SFA
</p>

<p>
“Apprentice” means those who receive apprenticeship training and (where 
required) end-point assessment through an apprenticeship framework or
standard funded by the Skills Funding Agency.
</p>

<p>
“Apprenticeship” means a job with an accompanying skills development
programme. This includes the training and (where required) end-point-
assessment for an employee as part of a job with an accompanying skills
development programme.
</p>

<p>
“Apprenticeship Framework” means [AGREE DEFINITION}
</p>

<p>
“Apprenticeship Levy” means a levy on UK employers to fund new
apprenticeships. In England, control of apprenticeship funding will be put in
the hands of employers through the Digital Apprenticeship Service
</p>

<p>
“Apprenticeship Standard” means [AGREE DEFINITION}
</p>

<p>
“Commitment Statement” means the details of Apprenticeship Standards or
Apprenticeship Frameworks which the Provider will deliver and the price
agreed for that delivery that is recorded on DAS
</p>

<p>
“Contract” means the Contract between the Employer and the Provider for the
delivery of Apprenticeships.
</p>

<p>
“Digital apprenticeship service” means the digital interface to services
designed to support the uptake of apprenticeships. The service is aimed
primarily at employers, with information coming from a range of different
sources including learning providers and apprenticeship assessment
organisations.
</p>

<p>
“Funding” means
</p>

<p>
“Funding Rules” means the document which sets out the detailed
requirements for Apprenticeships which the Employer and Provider must
comply with in order for the Provider to be funded by the SFA.
</p>

<p>
“Provider” means an organisation body who has a Contract with the Employer
to deliver Apprenticeships.
</p>

<p>
“Provider Agreement” means the agreement between the Provider and the
SFA setting out the condition on which the Provider will receive Funding from
the SFA.
</p>

<p>
“Register” means the Register of Apprenticeship Training Providers on which
all Providers must appear and the Register of Apprenticeship Assessment
Organisations on which all Assessment Organisations must appear.
</p>

<p>
3 Term of the Agreement
</p>

<p>
3.1 The Agreement will commence on XXXXX and shall end on XXXXX 
</p>


<p>
1 Introduction
</p>

<p>
1.1 This Agreement is made between the Secretary of State for Education
through the Skills Funding Agency, an executive agency of the Department for
Education (the SFA) and [Name of employer], (the Employer).
</p>

<p>
1.2 The Agreement applies to any organisation that employs Apprentices and
sets out the terms and conditions on which the SFA will make payments to
Providers who have a Contract with the Employer to deliver Apprenticeships.
</p>

<p>
2 Definitions
</p>

<p>
“Agreement” means this agreement between the Employer and the SFA
</p>

<p>
“Apprentice” means those who receive apprenticeship training and (where 
required) end-point assessment through an apprenticeship framework or
standard funded by the Skills Funding Agency.
</p>

<p>
“Apprenticeship” means a job with an accompanying skills development
programme. This includes the training and (where required) end-point-
assessment for an employee as part of a job with an accompanying skills
development programme.
</p>

<p>
“Apprenticeship Framework” means [AGREE DEFINITION}
</p>

<p>
“Apprenticeship Levy” means a levy on UK employers to fund new
apprenticeships. In England, control of apprenticeship funding will be put in
the hands of employers through the Digital Apprenticeship Service
</p>

<p>
“Apprenticeship Standard” means [AGREE DEFINITION}
</p>

<p>
“Commitment Statement” means the details of Apprenticeship Standards or
Apprenticeship Frameworks which the Provider will deliver and the price
agreed for that delivery that is recorded on DAS
</p>

<p>
“Contract” means the Contract between the Employer and the Provider for the
delivery of Apprenticeships.
</p>

<p>
“Digital apprenticeship service” means the digital interface to services
designed to support the uptake of apprenticeships. The service is aimed
primarily at employers, with information coming from a range of different
sources including learning providers and apprenticeship assessment
organisations.
</p>

<p>
“Funding” means
</p>

<p>
“Funding Rules” means the document which sets out the detailed
requirements for Apprenticeships which the Employer and Provider must
comply with in order for the Provider to be funded by the SFA.
</p>

<p>
“Provider” means an organisation body who has a Contract with the Employer
to deliver Apprenticeships.
</p>

<p>
“Provider Agreement” means the agreement between the Provider and the
SFA setting out the condition on which the Provider will receive Funding from
the SFA.
</p>

<p>
“Register” means the Register of Apprenticeship Training Providers on which
all Providers must appear and the Register of Apprenticeship Assessment
Organisations on which all Assessment Organisations must appear.
</p>

<p>
3 Term of the Agreement
</p>

<p>
3.1 The Agreement will commence on XXXXX and shall end on XXXXX 
</p>', GETDATE(), 'SFA Employer Agreement V1.0BETA', GETDATE()) 
END 


SET IDENTITY_INSERT  [account].[EmployerAgreementTemplate] OFF
