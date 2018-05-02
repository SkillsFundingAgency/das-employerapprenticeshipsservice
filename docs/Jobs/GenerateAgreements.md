# Digital Apprenticeships Service

## Employer Apprenticeship Service

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Employer Apprenticeship Service|

## Generate Agreements Job

### Purpose
This job will generate an employer agreement for all legal entities/accounts which are not already linked to the latest agreement.  


## How does it achieve this?

### In English...
It finds the latest agreement template Id and then scans the database looking for all legal entities that do not already have this agreement (whether signed or not). For each legal entity it finds it will create an agreement, generating an Agreement Created event. 

### In Nerd...
The class GenerateAgreementsIdProvider will identify the latest agreement using the IEmployerAgreementRepository.GetLatestAgreementTemplate() method. It will then return batches of legal entity ids that do not already have this agreement using the method ILegalEntityRepository.GetLegalEntitiesWithoutSpecificAgreement. Each legal entity id found is passed to GenerateAgreementsIdProcessor.DoAsync(...) method which will then find all accounts linked to the legal entity#1 and for each legal entity and account combination it will invoke the CreateEmployerAgreementCommand which, in turn, will call EmployerAgreementRepository.CreateEmployerAgreeement. This will do the usual stuff when an agreement is created, including raising the appropriate event to create the task.

## How do I run this
Post an ad-hoc job message specifying a job name of **GenerateAgreementsJob**. See [Running Jobs](RunningJobs.md "Running jobs")

## I'm worried - how can I tell what it's doing?

In the words of *Darth Vader*
> I find your lack of faith disturbing...

That aside, the following messages will be written to [Kibana](https://dev-elk.fasst.org.uk/login?next=%2Fapp%2Fkibana#?_g=(), "Kibana logs").


    Starting processing of accounts GenerateAgreementsIdProvider
    
    Finished processing for GenerateAgreementsIdProvider (some additional stats and information will be logged as well)
    

There are no intervening log messages unless something fails.


## Can I check in the database if something is happening?


You can run the following query. 


    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
    
    DECLARE @latestTemplateId  as int
    SELECT @latestTemplateId = MAX(Id) FROM employer_account.EmployerAgreementTemplate
    
    SELECT	DistinctLegalEntities as [Total Legal Entities],
    		WithLatestAgreement AS [Number that have latest agreement],
    		Convert(Decimal(6,2),CONVERT(Decimal(6,2), WithLatestAgreement) / Convert(Decimal(6,2), DistinctLegalEntities) * 100 ) AS [% Complete]
    FROM (
    	SELECT COUNT(*) as DistinctLegalEntities,
    		   SUM(HasLatest) as WithLatestAgreement
    	FROM (	 
    			SELECT	EA.LegalEntityId,
    					CASE WHEN Exists(SELECT 1 FROM  employer_account.EmployerAgreement WHERE LegalEntityId = EA.LegalEntityId AND TemplateId = @latestTemplateId) THEN 1 ELSE 0 END AS HasLatest
    			FROM	(SELECT DISTINCT LegalEntityId FROM employer_account.EmployerAgreement) AS EA
    		) AS T1
    ) AS T2


The above sql will generate a result that looks like this:

![LegalEntityStatus](LegalEntityStatus.PNG)

Hopefully you will see the percentage go up over time!

----------------
/#1 okay, in reality the application does not support a legal entity belonging to more than one account. However, the database does and the code must match the database.

