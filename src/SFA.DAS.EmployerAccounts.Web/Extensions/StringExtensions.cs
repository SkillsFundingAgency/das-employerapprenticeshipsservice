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

    public static bool IsEquivalent(this string value1, string value2, StringEquivalenceOptions options)
    {
        int index1 = 0, index2 = 0;

        if (string.Equals(value1, value2))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(value1) || string.IsNullOrWhiteSpace(value2))
        {
            return false;
        }

        if (options.HasFlag(StringEquivalenceOptions.IgnoreLeadingSpaces))
        {
            SkipToNextNonSpace(value1, ref index1);
            SkipToNextNonSpace(value2, ref index2);
        }

        var caseInsensitive = options.HasFlag(StringEquivalenceOptions.CaseInsensitive);

        while (index1 < value1.Length && index2 < value2.Length)
        {
            if (options.HasFlag(StringEquivalenceOptions.MultipleSpacesAreEquivalent))
            {
                var whiteSpace1 = JumpOverWhiteSpace(value1, ref index1);
                var whiteSpace2 = JumpOverWhiteSpace(value2, ref index2);

                if (whiteSpace1 ^ whiteSpace2)
                {
                    return false;
                }
            }

            if (!((caseInsensitive || value1[index1] == value2[index2]) && (!caseInsensitive || char.ToUpperInvariant(value1[index1]) == char.ToUpperInvariant(value2[index2]))))
            {
                return false;
            }

            index1++;
            index2++;
        }

        if (!options.HasFlag(StringEquivalenceOptions.IgnoreTrailingSpaces))
        {
            return index1 == value1.Length && index2 == value2.Length;
        }

        SkipToNextNonSpace(value1, ref index1);
        SkipToNextNonSpace(value2, ref index2);

        return index1 == value1.Length && index2 == value2.Length;
    }

    /// <summary>
    ///     Skips idx to the next non-white space position and returns true if this could
    ///     be done without running off the end of string.
    /// </summary>
    private static void SkipToNextNonSpace(string value, ref int index)
    {
        while (index < value.Length && char.IsWhiteSpace(value[index]))
            index++;
    }

    /// <summary>
    ///     Skips idx to the next non-white space position and returns true if any space
    ///     characters were skipped.
    /// </summary>
    private static bool JumpOverWhiteSpace(string value, ref int index)
    {
        if (index >= value.Length || !char.IsWhiteSpace(value[index]))
        {
            return false;
        }

        while (index < value.Length && char.IsWhiteSpace(value[index]))
            index++;

        return true;
    }
}