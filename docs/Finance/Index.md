# Digital Apprenticeships Service

## Employer Apprenticeship Service

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Employer Apprenticeship Service|

## Finance

### Purpose
This page describes the finance side of the repo and has links to subsections that describe things in more detail.  


### Database

     

## Sub Sections

* [Levy Declarations](LevyDeclarations/Index.md "Levy Declarations")


## Finance

As part of the Payment processing - there is a dependency on [das-apim-endpoints](https://github.com/SkillsFundingAgency/das-apim-endpoints) for getting Course and Provider information, as part of the PaymentMetadata information that is stored on the Transaction Line for the account. In the Employer Finance configuration the following is required:

```
"ManageApprenticeshipsOuterApiConfiguration":{
    "BaseUrl": "https://APIM-BASE-URL/",
    "Key": "APIM-KEY"
  },
```

You can request a subscription key if you are part of the ESFA organisation, if not information about running the outer api is provided on the das-apim-endpoints readme.
