open Fake

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
            { p with
                Project = ""})
)
