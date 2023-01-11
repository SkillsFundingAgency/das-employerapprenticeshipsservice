namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public enum StringEquivalenceOptions
{
    None = 0,

    // Leading spaces will be ignored when comparing strings
    IgnoreLeadingSpaces = 1,

    // Trailing spaces will be ignored when comparing strings
    IgnoreTrailingSpaces = 2,

    // Spaces within the string will be considered the same
    // regardless of how many spaces there are
    MultipleSpacesAreEquivalent = 4,

    // Casing and culture will be ignored when comparing strings
    CaseInsensitive = 8,

    Default = IgnoreLeadingSpaces | IgnoreTrailingSpaces | MultipleSpacesAreEquivalent | CaseInsensitive
}

public static class StringExtensions
{
    public static string FormatPayeForUrl(this string scheme)
    {
        return scheme.Replace("/", "_");
    }

    public static string FormatPayeFromUrl(this string scheme)
    {
        return scheme.Replace("_", "/");
    }

    public static bool IsEquivalent(this string s1, string s2, StringEquivalenceOptions options)
    {
        int idx1 = 0, idx2 = 0;

        if (string.Equals(s1, s2))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(s1) || string.IsNullOrWhiteSpace(s2))
        {
            return false;
        }

        if (options.HasFlag(StringEquivalenceOptions.IgnoreLeadingSpaces))
        {
            SkipToNextNonSpace(s1, ref idx1);
            SkipToNextNonSpace(s2, ref idx2);
        }

        var caseInsensitive = options.HasFlag(StringEquivalenceOptions.CaseInsensitive);

        while (idx1 < s1.Length && idx2 < s2.Length)
        {
            if (options.HasFlag(StringEquivalenceOptions.MultipleSpacesAreEquivalent))
            {
                var ws1 = JumpOverWhiteSpace(s1, ref idx1);
                var ws2 = JumpOverWhiteSpace(s2, ref idx2);

                if (ws1 ^ ws2)
                {
                    return false;
                }
            }

            if (!((caseInsensitive || s1[idx1] == s2[idx2]) && (!caseInsensitive || char.ToUpperInvariant(s1[idx1]) == char.ToUpperInvariant(s2[idx2]))))
            {
                return false;
            }

            idx1++;
            idx2++;
        }

        if (!options.HasFlag(StringEquivalenceOptions.IgnoreTrailingSpaces))
        {
            return idx1 == s1.Length && idx2 == s2.Length;
        }

        SkipToNextNonSpace(s1, ref idx1);
        SkipToNextNonSpace(s2, ref idx2);

        return idx1 == s1.Length && idx2 == s2.Length;
    }

    /// <summary>
    ///     Skips idx to the next non-white space position and returns true if this could
    ///     be done without running off the end of string.
    /// </summary>
    private static void SkipToNextNonSpace(string s, ref int idx)
    {
        while (idx < s.Length && char.IsWhiteSpace(s[idx]))
            idx++;
    }

    /// <summary>
    ///     Skips idx to the next non-white space position and returns true if any space
    ///     characters were skipped.
    /// </summary>
    private static bool JumpOverWhiteSpace(string s, ref int idx)
    {
        if (idx >= s.Length || !char.IsWhiteSpace(s[idx]))
        {
            return false;
        }

        while (idx < s.Length && char.IsWhiteSpace(s[idx]))
            idx++;

        return true;
    }
}