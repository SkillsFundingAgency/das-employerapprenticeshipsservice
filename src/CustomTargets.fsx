open Fake

let testDirectory = getBuildParamOrDefault "buildMode" "Debug"
let nUnitRunner = "nunit3-console.exe"
let mutable nUnitToolPath = @"tools\NUnit.ConsoleRunner\"
let acceptanceTestPlayList = getBuildParamOrDefault "playList" ""
let nunitTestFormat = getBuildParamOrDefault "nunitTestFormat" "nunit2"
let rootPublishDirectory = getBuildParamOrDefault "publishDirectory"  @"C:\CompiledSource"
let mutable projectName = ""
let mutable publishDirectory = rootPublishDirectory @@ projectName

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.UnitTests" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts.Host" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts.Jobs" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts.MessageHandlers" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts.Messages" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts.Web" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerAccounts.Web.UnitTests" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance.Host" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance.Jobs" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance.MessageHandlers" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance.Messages" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance.Web" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance.MessageHandlers.UnitTests" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EmployerFinance.Web.UnitTests" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Account.Api.Client" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EAS.Account.Api.Types" })
)

Target "Build And Zip Webjob Host Projects" ( fun _ ->
    let buildMode = getBuildParamOrDefault "buildMode" "Debug"

    if buildMode.ToLower().Equals("release") then
        let directoryinfo = FileSystemHelper.directoryInfo(@".\" @@ publishDirectory @@ "\..\Host")
        let directory = directoryinfo.FullName
        traceImportant directory
        let properties =
                        [
                            ("DeployOnBuild", "True");
                            ("WebPublishMethod", "Package");
                            ("PackageAsSingleFile", "True");
                            ("SkipInvalidConfigurations", "true");
                            ("PackageLocation", directory);
                            ("ToolsVersion","14");
                        ]

        !! (@".\**\*.Host.csproj")
            |> MSBuildReleaseExt null properties "Build"
            |> Log "Build-Output: "
)

Target "Build And Zip Web App Projects" ( fun _ ->
    let buildMode = getBuildParamOrDefault "buildMode" "Debug"

    if buildMode.ToLower().Equals("release") then
        let directoryinfo = FileSystemHelper.directoryInfo(@".\" @@ publishDirectory @@ "\..\WebApps")
        let directory = directoryinfo.FullName
        traceImportant directory
        let properties =
                        [
                            ("DeployOnBuild", "True");
                            ("WebPublishMethod", "Package");
                            ("PackageAsSingleFile", "True");
                            ("SkipInvalidConfigurations", "true");
                            ("PackageLocation", directory);
                            ("ToolsVersion","14");
                        ]

        !! (@".\**\SFA.DAS.EmployerFinance.Web.csproj")
            |> MSBuildReleaseExt null properties "Build"
            |> Log "Build-Output: "

        !! (@".\**\SFA.DAS.EmployerAccounts.Web.csproj")
        |> MSBuildReleaseExt null properties "Build"
        |> Log "Build-Output: "
)