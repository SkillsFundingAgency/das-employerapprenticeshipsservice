# the following will allow multiple csdef files by swapping in a copy and renaming it on build. For VSTS.
# add the following line as a pre build step
# if $(ConfigurationName) == Release (powershell "start-process powershell.exe, '$(ProjectDir)PreDeployment.ps1', '$(ProjectDir)', '$(TargetDir)'")

param (
[string]$ProjectDir,
[string]$TargetDir
)

if(-not ($Env:EnvironmentName))
{
    Write-Host "Missing variable. Either this is not a build server or the variable is not set"
    exit 1
}

if ($Env:EnvironmentName -eq 'PROD')
{
    #Replace the csdef file 
    Write-Host "$Env:EnvironmentName"
    Copy-Item ($ProjectDir+"\ServiceDefinition.Release.csdef") ($ProjectDir+"\ServiceDefinition-copy.csdef") -Force 
    Write-Host "Service Definition file replaced"
    exit 1
}