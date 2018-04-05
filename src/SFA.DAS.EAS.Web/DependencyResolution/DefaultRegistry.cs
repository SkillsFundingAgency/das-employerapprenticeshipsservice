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

using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using AutoMapper;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Audit.Client;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Factories;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Infrastructure.Services.Features;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Tasks.API.Client;
using StructureMap;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using WebGrease.Css.Extensions;

namespace SFA.DAS.EAS.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
            For<IControllerMetaDataService>().Use(() => new ControllerMetaDataService(typeof(HomeController).Assembly)).Singleton();
        }
    }
}