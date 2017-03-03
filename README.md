# Employer Apprenticeships Service (BETA)

This solution represents the Employer Apprenticeships Service (currently pre private BETA) code base.

### Build
![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/101/badge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Account.Api.Client)](https://www.nuget.org/packages/SFA.DAS.Account.Api.Client)

## Running locally

### Requirements

In order to run this solution locally you will need the following installed:

* [SQL Server LocalDB](https://www.microsoft.com/en-us/download/details.aspx?id=52679) - you only need the LocalDB component of SQL Server Express
* [Azure SDK 2.9 (for Visual Studio)](https://azure.microsoft.com/en-us/downloads/) - choose the SDK version appropriate to your version of Visual Studio
* [Azure Storage Explorer](http://storageexplorer.com/)
* [SFA.DAS.EmployerUsers IDAMS](https://github.com/SkillsFundingAgency/das-employerusers)

You should run Visual Studio as an Administrator.

### Setup

* Run DevInstall.ps1 in an Administrator PowerShell window
* Running the RunBuild.bat ([FAKEBuildScript](https://www.nuget.org/packages/FAKEBuildScript)) will expose any dependency issues and also download all required packages
* Publishing the Database projects (SFA.DAS.EAS.Employer_Account.Database and SFA.DAS.EAS.Employer_Financial.Database) will provision the local DB instance with relevent seed data.
* In the Azure Storage Explorer, connect to local storage. In the "(Development)" storage account, create a new local table named "Configuration" and load the following row data:
  * PartitionKey => LOCAL
  * RowKey => SFA.DAS.EmployerApprenticeshipsService_1.0
  * Data => Contents of the environment configuration JSON document (found in src/SFA.DAS.EmployerApprenticeshipsService.Web\App_Data\LOCAL_SFA.DAS.EmployerApprenticeshipsService.Web_1.0.json). Please note this does not contain any keys required for API integration.
* Update the connection string in the JSON to point at your LocalDB instance (look around in Visual Studio's SQL Server Object Explorer)

### Running

Running the cloud service will start the web application in your browser.

### Feature Toggle

You can limit areas of the site by adding them to a list, in the controller action format, or having a * to denote all actions within that controller. Below is an example of the config required:

```
{   "Data": [     {       "Controller": "EmployerTeam",       "Action": "Invite"     }   ] }
```
This is added to the configuration table of your local azure storage, with the PartiionKey being **LOCAL** and the RowKey being **SFA.DAS.EmployerApprenticeshipsService.Features_1.0**

## Account API
The Employer Apprenticeships Service provides a REST Api and client for accessing Employer accounts. Nuget link above.

* The API can be found in [src/SFA.DAS.EAS.Api](src/SFA.DAS.EAS.Api)
* The client can be found in [src/SFA.DAS.Account.Api.Client](src/SFA.DAS.Account.Api.Client)
