# Employer Apprenticeships Service (BETA)

## Running locally

### Requirements

In order to run this solution locally you will need the following installed:

* SQL Server LocalDB
* Azure SDK 2.9
* Azure Storage Explorer

### Setup

* Running the RunBuild.bat will expose any dependeny issues
* Running the Database project (SFA.DAS.EmployerApprenticeshipsService.Database) will provision the local DB instance with relevent seed data.
* In the Azure Storage Explorer create a new local table named "Configuration" and load the following row data:
  * PartitionKey => LOCAL
  * RowKey => SFA.DAS.EmployerApprenticeshipsService_1.0
  * Data => Contents of the environment configuration JSON document (see private configuration repository)

### Running

Running the cloud service will start the web application in your browser.
