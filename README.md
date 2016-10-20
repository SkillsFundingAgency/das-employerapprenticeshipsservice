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


### Whitelisting

To limit access to the site to subset of users you can add regex patterns to allow users with certain email addresses to access the site only. Unlisted email addresses will be redirected to a 'user not allowed' page but will still be able to create a user account and log in.

To add such regex patterns simple open up the json file:

**SFA.DAS.EmployerApprenticeshipsService.Web/App_Data/WhiteList/user_white_list.json**

And add any number of regex patterns you wish to use to select users by email for the site's whitelist.

Below is an example of what the file may look like

```
{
  "EmailPatterns": [
    "^[a-zA-Z0-9.-]*@test.com$",
    "^test2@test.com$",
    "test@[a-z]*.co.uk"
  ]
}
```

### Feature Toggle

You can limit areas of the site by adding them to a list, in the controller action format, or having a * to denote all actions within that controller. Below is an example of the config required:

```
{   "Data": [     {       "Controller": "EmployerTeam",       "Action": "Invite"     }   ] }
```
This is added to the configuration table of your local azure storage, with the PartiionKey being **LOCAL** and the RowKey being **SFA.DAS.EmployerApprenticeshipsService.Features_1.0**

### Managed Companies House List

You are able to define your own set of companies that aren't available through the companies house API. the Key _UseManagedList_ needs to be set to true then the following entires in the Configuration table in the Azure Storage

```
{   "Data": [     {   "company_name": "Non companies house company",   "company_number": "AML-456789",   "date_of_creation": "2001-10-01T00:00:00",   "registered_office_address": {         "address_line_1": "Test Address",         "address_line_2": "Test",         "postal_code": "T31 EST"       } }   ] }
```
The PartitionKey is **LOCAL** and the RowKey is **SFA.DAS.EmployerApprenticeshipsService.CompanyLookup_1.0**

## Account API
The Employer Apprenticeships Service provides a REST Api and client for accessing Employer accounts.

* The API can be found in [src/SFA.DAS.EAS.Api](src/SFA.DAS.EAS.Api)
* The client can be found in [src/SFA.DAS.Account.Api.Client](src/SFA.DAS.Account.Api.Client)
