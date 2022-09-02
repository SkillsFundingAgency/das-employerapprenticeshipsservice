﻿using AutoMapper;
using Castle.Core.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    public abstract class AccountTransactionsControllerTests
    {
        protected IMapper Mapper;

        protected IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load($"SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}