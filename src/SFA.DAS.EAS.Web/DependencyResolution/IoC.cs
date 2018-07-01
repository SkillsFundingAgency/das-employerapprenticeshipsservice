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

namespace SFA.DAS.EAS.Web.DependencyResolution
{
    using SFA.DAS.Activities.Client;
    using SFA.DAS.EAS.Application.DependencyResolution;
    using StructureMap;
	
    public static class IoC {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<ActivitiesClientRegistry>();
                c.AddRegistry<ApprenticeshipLevyRegistry>();
                c.AddRegistry<AuditRegistry>();
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LevyRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<NotificationsRegistry>();
                c.AddRegistry<NServiceBusRegistry>();
                c.AddRegistry<ReferenceDataRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<ServicesRegistry>();
                c.AddRegistry<TasksRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<ValidationRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}