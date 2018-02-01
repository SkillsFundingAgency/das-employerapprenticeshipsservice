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


### Integration Testing
#### Purpose
- __Integration Tests__ run code to test for expected results. It will run a single item (usually an end point) using no mocks and run with real data.
- __Unit tests__ also run code but will use mocks/fakes and won't use real data.
- __Acceptance tests__ also run code but will typically run end-to-end to cover a feature or user journey.

The intention of the integration test is to verify that the individual end points work in a realistic environment. Integration tests usually have a dependency on actual data.

#### Writing Unit Tests
 
Integration tests are written in the same way as unit tests (i.e. using nunit and a suitable test runner). The  difference is that the test setup does not use mocks. Below is an example of a simple integration test that calls the __api/Healthcheck__ end point and expects a status code of __OK__. 

        [Test]
        public async Task ThenTheStatusShouldBeOk()
        {
            // Arrange
            var call = new CallRequirements("api/HealthCheck")
                            .AllowStatusCodes(HttpStatusCode.OK);

            // Act
            await ApiIntegrationTester.InvokeIsolatedGetAsync(call);

            // Assert
            Assert.Pass("Verified we got OK");
        }

#####Arrange#####
The test sets up the requirements using a new instance of a `CallRequirements()` object. Once this object is the `AllowStatusCodes()` extension method is used to specify the status codes that indicate that the call worked okay - in this case only the status code __OK__ is acceptable. All other status codes indicate that an error has occurred.

Other extension methods allow other requirements to be set. For example:

- ExpectControllerType
- ExpectValidationError
- IgnoreExceptionTypes

#####Act#####
Here, the call is made by passing the `CallRequirements` created previously to the `InvokeIsolatedGetAsync()` method. This is a static method, so we don't need an instance of the ApiIntegrationTester. In this example the test is only interested in the status code - there is no response body here so no further checks are required.

If the response body is expected to return something then it can be obtained by using the generic version of the Invoke method as follows:

            var legalEntities = await _tester.InvokeIsolatedGetAsync<ResourceList>(callRequirements);

In this example the invoke method will return an instance of `ResourceList` (which may be null if the response body was blank).



> In the previous examples the InvokeIsolatedGetAsync method was used. This is great for running a single test but starts up an in memory web server, with all the configuration from the web API project so takes quite a long time to get started. If you're going to run multiple tests then it is better to use the non-isolated get methods. 

######Non-Isolated Invoke Methods

These have the same name and signatures as the IsolatedInvoke methods but instead of being static methods these are instance methods, so an instance of `ApiIntegrationTester` will be required. This should be created in the test class's start up method and then disposed in the corresponding tear down method. The instance can then be shared with the individual test methods in the normal way.
 
#####Assert#####

These can do whatever asserts are appropriate for the test. 
Note that you do not need to test:

- the status code is the expected status code - *always tested*
- whether the web server threw an unhandled exception - *always tested*
- whether the expected controller was created - * only if the call specified an expected controller*
- whether the returned body can be deserialised into the required type correctly - *only if the generic Invoke method was used*

The above will all be validated automatically - there is no need to validate these explicitly.  

###Building Test Data

You can use the `APIIntegrationTester` to setup the required data for the test. This will write data to the actual database configured for the current environment.
To do this you will need to use the `DbBuilder` property on the `ApiIntegrationTester` instance.

            var builder = _tester.DbBuilder;

Once you have this you can call methods to set update, like so:

            builder
                .EnsureUserExists(builder.BuildUserInput())
                .EnsureAccountExists(builder.BuildEmployerAccountInput(accountName))
                .WithLegalEntity(builder.BuildEntityWithAgreementInput(legalEntityName));

The builder methods (`EnsureUserExists` etc) accept an input message. Some of these are quite big, so there are helper extension methods (such as `BuildEmployeeAccountInput`) that can be used to create these.

As each method is used to create data details of the created data are stored and then used in subsequent calls. So in the above example:


- the user created by the `EnsureUserExists`() will be the user used in all subsequent calls until the next call to `EnsureUserExists`().
- the account created by the `EnsureAccountExists`() will the account used in all subsequent calls until the next call to `EnsureAccountExists`().      

> You can get access to the current context by using the DbBuilderContext property on the DbBuilder, like so:
> 
>             var hashedAccountId = builder.Context.ActiveEmployerAccount.HashedAccountId;

 


  