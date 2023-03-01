using System;
using FluentAssertions;

namespace SFA.DAS.EAS.TestCommon.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsEquivalentTo(this object source, object expectation)
        {
            try
            {
                source.ShouldBeEquivalentTo(expectation);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}