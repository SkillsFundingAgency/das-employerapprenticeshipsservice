open Fake

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Account.Api.Client" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.EAS.Account.Api.Types" })
)
