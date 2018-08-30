using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Extensions;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Extensions
{
    [TestFixture]
    public class StringEquivalenceOptionsTests
    {
        [TestCase("abc", "abc", StringEquivalenceOptions.Default, true)]
        [TestCase("abc", "ABC", StringEquivalenceOptions.CaseInsensitive, true)]
        [TestCase("abc", "ABC", StringEquivalenceOptions.None, false)]
        [TestCase(" abc", "abc", StringEquivalenceOptions.IgnoreLeadingSpaces, true)]
        [TestCase("abc  ", "abc", StringEquivalenceOptions.IgnoreTrailingSpaces, true)]
        [TestCase("abc  ", "abc", StringEquivalenceOptions.None, false)]
        [TestCase("abc def", "abc def", StringEquivalenceOptions.Default, true)]
        [TestCase("abc   def", "abc def", StringEquivalenceOptions.Default, true)]
        [TestCase("abc   def", "abc def", StringEquivalenceOptions.None, false)]
        [TestCase("abc def", "abcdef", StringEquivalenceOptions.Default, false)]
        [TestCase("abc  def", "abcdef", StringEquivalenceOptions.Default, false)]
        [TestCase("abc  def  ", "abcdef", StringEquivalenceOptions.Default, false)]
        [TestCase("abc  def  ", "abcdef  ", StringEquivalenceOptions.Default, false)]
        [TestCase("abc  def", "abcdef  ", StringEquivalenceOptions.Default, false)]
        [TestCase("", "", StringEquivalenceOptions.Default, true)]
        [TestCase(null, null, StringEquivalenceOptions.Default, true)]
        [TestCase("abc", null, StringEquivalenceOptions.Default, false)]
        [TestCase("abc", "", StringEquivalenceOptions.Default, false)]
        [TestCase("a b c d", "a  b  c  d", StringEquivalenceOptions.Default, true)]
        [TestCase("aaa bbb ccc ddd", "aaa  bbb  ccc  ddd", StringEquivalenceOptions.Default, true)]
        public void IsEquivalent_GivenValues_ReturnsExpectedResult(
            string s1, 
            string s2,
            StringEquivalenceOptions options, 
            bool expectedValue)
        {
            var actualResult1 = s1.IsEquivalent(s2, options);
            Assert.AreEqual(expectedValue, actualResult1, $"\"{s1}\".IsEquivalant(\"{s2}\", {options}) ");

            // the order of the strings shouldn't matter, so we'll switch them round and do the test the other way
            var actualResult2 = s2.IsEquivalent(s1, options);
            Assert.AreEqual(expectedValue, actualResult2, $"\"{s2}\".IsEquivalant(\"{s1}\", {options}) ");
        }
    }
}
