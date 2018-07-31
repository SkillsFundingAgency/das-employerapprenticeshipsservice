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