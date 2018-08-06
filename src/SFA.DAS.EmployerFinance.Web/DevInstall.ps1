#Requires -Version 3.0
#Requires -RunAsAdministrator

Set-StrictMode -Version Latest

$projectName = "SFA.DAS.EmployerFinance.Web"
$port = "44304"

$appDataPath = "$Env:APPDATA"
$programFilesPath = "$Env:PROGRAMFILES"
$projectPath = "$PSScriptRoot"
$configPath = "$appDataPath\$projectName"
$applicationHostConfigFilename = "ApplicationHost.config"
$applicationHostConfigSourcePath = "$programFilesPath\IIS Express\AppServer\$applicationHostConfigFilename"
$applicationHostConfigDestinationPath = "$configPath\$applicationHostConfigFilename"
$appCmdExe = "$programFilesPath\IIS Express\appcmd.exe"
$iisExpressAdminCmdExe = "$programFilesPath\IIS Express\IisExpressAdminCmd.exe"

md "$configPath" -force
copy "$applicationHostConfigSourcePath" "$configPath"

& $appCmdExe set config -section:system.applicationHost/sites /"[name='Development Web Site'].[path='/'].[path='/'].physicalPath:${projectPath}" /commit:apphost /apphostconfig:"$applicationHostConfigDestinationPath"
& $appCmdExe set config -section:system.applicationHost/sites /+"[name='Development Web Site'].bindings.[protocol='https',bindingInformation=':${port}:localhost']" /commit:apphost /apphostconfig:"$applicationHostConfigDestinationPath"
& $iisExpressAdminCmdExe setupSslUrl -url:https://localhost:$port/ -UseSelfSigned