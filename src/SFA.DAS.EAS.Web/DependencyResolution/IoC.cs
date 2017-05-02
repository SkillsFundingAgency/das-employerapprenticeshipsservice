// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
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


using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Web.DependencyResolution
{
    public static class IoC
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";

        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(ServiceName));
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                c.Policies.Add(new ConfigurationPolicy<NotificationsApiClientConfiguration>($"{ServiceName}.Notifications"));
                c.Policies.Add(new ConfigurationPolicy<ReferenceDataApiClientConfiguration>("SFA.DAS.ReferenceDataApiClient"));
                c.Policies.Add(new ConfigurationPolicy<AuditApiClientConfiguration>("SFA.DAS.AuditApiClient"));
                c.Policies.Add(new ConfigurationPolicy<TokenServiceApiClientConfiguration>("SFA.DAS.TokenServiceApiClient"));
				c.Policies.Add(new ConfigurationPolicy<CommitmentsApiClientConfiguration>("SFA.DAS.CommitmentsAPI"));
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add<CurrentDatePolicy>();
                c.Policies.Add(new MessagePolicy<EmployerApprenticeshipsServiceConfiguration>(ServiceName));
                c.Policies.Add(new ExecutionPolicyPolicy());
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}