# Employer Apprenticeships Service (BETA)

This solution represents the Employer Apprenticeships Service (currently pre private BETA) code base.

### Build
![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/101/badge)

## Running locally

### Requirements

In order to run this solution locally you will need the following installed:

* [SQL Server LocalDB](https://www.microsoft.com/en-us/download/details.aspx?id=52679) - you only need the LocalDB component of SQL Server Express
* [Azure SDK 2.9 (for Visual Studio)](https://azure.microsoft.com/en-us/downloads/) - choose the SDK version appropriate to your version of Visual Studio
* [Azure Storage Explorer](http://storageexplorer.com/)

You should run Visual Studio as an Administrator.

### Setup

* Run DevInstall.ps1 in an Administrator PowerShell window
* Running the RunBuild.bat will expose any dependency issues
* Running the Database project (SFA.DAS.EmployerApprenticeshipsService.Database) will provision the local DB instance with relevent seed data.
* In the Azure Storage Explorer, connect to local storage. In the "(Development)" storage account, create a new local table named "Configuration" and load the following row data:
  * PartitionKey => LOCAL
  * RowKey => SFA.DAS.EmployerApprenticeshipsService_1.0
  * Data => Contents of the environment configuration JSON document (found in src/SFA.DAS.EmployerApprenticeshipsService.Web\App_Data\LOCAL_SFA.DAS.EmployerApprenticeshipsService.Web_1.0.json)
* Update the connection string in the JSON to point at your LocalDB instance (look around in Visual Studio's SQL Server Object Explorer)
* If required, set "UseFake":"true" in the JSON

### Running

Running the cloud service will start the web application in your browser.
