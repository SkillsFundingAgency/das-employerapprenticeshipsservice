param (
[string]$ProjectDir,
[string]$TargetDir
)


    #Replace the csdef file 
    Write-Host "$Env:EnvironmentName"
    Copy-Item ($ProjectDir+"\ServiceDefinition.Release.csdef") ($ProjectDir+"\ServiceDefinition-copy2.csdef") -Force 
    Write-Host "Service Definition file replaced"
    exit 1
