open Fake

let testDirectory = getBuildParamOrDefault "buildMode" "Debug"
let nUnitRunner = "nunit3-console.exe"
let mutable nUnitToolPath = @"tools\NUnit.ConsoleRunner\"
let acceptanceTestPlayList = getBuildParamOrDefault "playList" ""
let nunitTestFormat = getBuildParamOrDefault "nunitTestFormat" "nunit2"

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Account.Api.Client" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EAS.Account.Api.Types" })
)

Target "Run EAS Acceptance Tests" (fun _ ->

    trace "Run EAS Acceptance Tests"

    let mutable shouldRunTests = false

    let testDlls = !! ("./**/bin/" + testDirectory + "/*.AcceptanceTests.dll")

    for testDll in testDlls do
        shouldRunTests <- true

    if shouldRunTests then
        testDlls |> Fake.Testing.NUnit3.NUnit3 (fun p ->
            {p with
                ToolPath = (nUnitToolPath + @"tools\" + nUnitRunner);
                StopOnError = false;
                Agents = Some 1;
                Testlist = acceptanceTestPlayList;
                ResultSpecs = [("TestResult.xml;format=" + nunitTestFormat)];
                })
)