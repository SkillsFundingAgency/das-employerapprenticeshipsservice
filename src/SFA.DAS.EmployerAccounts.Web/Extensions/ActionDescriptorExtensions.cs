﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class ActionDescriptorExtensions
    {
        private static readonly ConcurrentDictionary<string, object> Cache = new ConcurrentDictionary<string, object>();

        public static T GetCustomAttribute<T>(this ActionDescriptor actionDescriptor) where T : Attribute
        {
            return Cache.GetOrAdd(actionDescriptor.UniqueId, k => 
                actionDescriptor.GetCustomAttributes(typeof(T), true).SingleOrDefault() ??
                actionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(T), true).SingleOrDefault()) as T;
        }
    }
}