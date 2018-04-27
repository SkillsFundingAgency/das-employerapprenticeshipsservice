# Digital Apprenticeships Service

## Employer Apprenticeship Service

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Employer Apprenticeship Service|
| Build | ![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/101/badge) |
| Web  | https://manage-apprenticeships.service.gov.uk/  |

## Account Api

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)| Account API |
| Client  | [![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Account.Api.Client)](https://www.nuget.org/packages/SFA.DAS.Account.Api.Client)  |


### Developer Setup

#### Requirements

1. Install [Visual Studio] with these workloads:
    - ASP.NET and web development
    - Azure development
    - .NET desktop development
2. Install [SQL Server Management Studio]
3. Install [Azure Storage Explorer]
4. Administator Access

#### Setup

##### Install SSL certificates for HTTPS on IIS express

- Open PowerShell as an administrator
- Run src\DevInstall.ps1

##### Install Elastic Search

- Follow the readme at [SFA.DAS.Activities] to install [Elastic Search]

##### Open the solution

- Open Visual Studio as an administrator
- Open the solution
- Set SFA.DAS.CloudService as the startup project
- Running the solution will launch the site in your browser

##### Publish the databases

Repeat these steps for:

1. SFA.DAS.EAS.Employer_Account.Database
2. SFA.DAS.EAS.Employer_Financial.Database

Steps:

* Right click on the db project in the solution explorer
* Click on publish menu item
* Click the edit button

![Click the edit button](/docs/img/db1.PNG)

* Select Local > ProjectsV13

![Select Local > ProjectsV13](/docs/img/db2.PNG)

* Add the project name in again as the Database name (i.e. SFA.DAS.EAS.Employer_Account.Database)
* Click publish

![Select Local > ProjectsV13](/docs/img/db3.PNG)

**TODO replace the publish with a post deploy step on building**

##### Add configuration to Azure Storage Emulator

The configuration is loaded from azure table storage.

* Run the Azure Storage Emulator
* Clone the [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config) repository
* Clone the [das-employer-config-updater](https://github.com/SkillsFundingAgency/das-employer-config-updater) repository
* Run the das-employer-config-updater console application and follow the instructions to import the config from the das-employer-config directory

> The two repos above are private. If the links appear to be dead make sure you are logged into github with an account that has access to these (i.e. that you are part of the Skills Funding Agency Team organization).

  
Using `"Action": "*"` can also be used to disable all actions on the controller.

[Azure Storage Explorer]: http://storageexplorer.com/
[Choclatey]: https://chocolatey.org
[Docker]: https://www.docker.com
[Elastic Search]: https://www.elastic.co/products/elasticsearch
[SFA.DAS.Activities]: https://github.com/SkillsFundingAgency/das-activities/blob/master/README.md
[SQL Server Management Studio]: https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
[Visual Studio]: https://www.visualstudio.com

## See Also
[Integration Tests](IntegrationTesting.md)
[Authorization Pipeline](Authorization Pipeline.md)