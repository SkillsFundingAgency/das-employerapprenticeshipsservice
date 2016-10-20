# Account API .NET Client
This project provides a client for consuming the DAS Account API from .NET projects.

## Installing the Client
The client is published to nuget.org. You can install it using the following

```
Install-Package SFA.DAS.Account.Api.Client
```

## Required configuration
You will need to know the following information for the API endpoint you want to connect to:

* Base Url - This is the root url of the deployment, including at a minimum the schema and host; and optionally the port and application path. For example https://some-server/ or https://some-server:1234/some/sub/path/
* Client token - This is the JWT token that has been issued for your service to be used in a specific environment

Example:
```csharp
var configuration = new SFA.DAS.Account.Api.Client.AccountApiConfiguration
{
    ApiBaseUrl = "https://some-server/",
    ClientToken = "YOUR_SECURE_TOKEN"
};
```

## Creating a client

```csharp
var client = new SFA.DAS.Account.Api.Client.AccountApiClient(configuration);
```

## Getting a page of accounts
The api is paged and the client provides defaults.

You can get the first page or 1000 with:
```csharp
var page = await client.GetPageOfAccounts();
```
Or
```csharp
var page = await client.GetPageOfAccounts(1, 1000);
```

you can also specify you want the account balance up-to a certain point (thus ignoring and credits and debits after that point).

```csharp
var page = await client.GetPageOfAccounts(1, 1000, new DateTime(2016, 10, 1));
```

The time portion of the date is ignored if present
