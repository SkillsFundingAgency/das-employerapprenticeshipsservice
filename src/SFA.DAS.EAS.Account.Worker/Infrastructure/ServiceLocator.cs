using System;
using StructureMap;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
	public static class ServiceLocator
	{
		private static IContainer _container;

		public static void Initialise(IContainer container)
		{
			_container = container;
		}

		public static T Get<T>()
		{
			return _container.GetInstance<T>();
		}

		public static T Get<T>(Type type) where T : class
		{
			return _container.GetInstance(type) as T;
		}

		public static IContainer CreateNestedContainer()
		{
			return _container.GetNestedContainer();
		}
	}
}