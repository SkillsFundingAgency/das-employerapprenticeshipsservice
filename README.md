# Employer Apprenticeships Service (BETA)

This solution represents the Employer Apprenticeships Server (currently pre private BETA) code base.

## Running locally

### Requirements

In order to run this solution locally you will need the following installed:

* SQL Server LocalDB
* Azure SDK 2.9
* Azure Storage Explorer

You should run Visual Studio as an Administrator.

### Setup

* Running the RunBuild.bat will expose any dependency issues
* Running the Database project (SFA.DAS.EmployerApprenticeshipsService.Database) will provision the local DB instance with relevent seed data.
* In the Azure Storage Explorer create a new local table named "Configuration" and load the following row data:
  * PartitionKey => LOCAL
  * RowKey => SFA.DAS.EmployerApprenticeshipsService_1.0
  * Data => Contents of the environment configuration JSON document (see private configuration repository)

### Running

Running the cloud service will start the web application in your browser.

### Build
![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/101/badge)
