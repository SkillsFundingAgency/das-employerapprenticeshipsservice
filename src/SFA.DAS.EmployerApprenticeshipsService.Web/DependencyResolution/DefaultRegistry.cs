// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Web;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using StructureMap.Web.Pipeline;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.DependencyResolution {
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
	
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService")
                        && !a.GetName().Name.Equals("SFA.DAS.EmployerApprenticeshipsService.Infrastructure"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<IOwinWrapper>().Transient().Use(() => new OwinWrapper(HttpContext.Current.GetOwinContext())).SetLifecycleTo(new HttpContextLifecycle());
            //For<IExample>().Use<Example>();

            For<IUserRepository>().Use<FileSystemUserRepository>();
            For<IUserAccountRepository>().Use<UserAccountRepository>();
            For<IEmployerVerificationService>().Use<CompaniesHouseEmployerVerificationService>();
			AddMediatrRegistrations();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }

        #endregion
    }
}