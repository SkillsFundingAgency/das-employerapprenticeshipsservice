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


## Authorization Pipeline
### Purpose
A filter that determines whether the current user has access to a feature or not.



##Features
Config 

A feature is defined as an enum in the FeatureType enum. Once defined the feature can be:

* Added to an operation (i.e. method on a controller)
* Added to a controller
* Added to an agreement
* Added to a json config that allows a whitelist to be defined

The json config has changed from 
 
```JavaScript
{
    "Data": [{
        "Controller": "EmployerTeam",
        "Action": "Invite",
        "Whitelist": ["john.doe@ma.local"]
    }]
}
```

to

```JavaScript
{
    "Data": [{
		"FeatureType": "<value from enum",
		"Name": "...",
		"Enabled":  "true"|"false",
		"WhiteList": ["...", "...", "..."]
		"EnabledByAgreementVersion": 2
    }]
}
```
 

  