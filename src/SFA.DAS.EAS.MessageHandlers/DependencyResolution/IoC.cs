﻿using StructureMap;

namespace SFA.DAS.EAS.MessageHandlers.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}