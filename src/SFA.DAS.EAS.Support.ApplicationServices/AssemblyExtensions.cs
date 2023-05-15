using System.Diagnostics;
using System.Reflection;

namespace SFA.DAS.EAS.Support.ApplicationServices;

public static class AssemblyExtensions
{
    public static string Version(this Assembly source)
    {
        var fvi = FileVersionInfo.GetVersionInfo(source.Location);
        return fvi.FileVersion;
    }
}