#Requires -Version 3.0
#Requires -RunAsAdministrator

Set-StrictMode -Version Latest

$port = "44344"
$programFilesPath = "$Env:PROGRAMFILES"
$iisExpressAdminCmdExe = "$programFilesPath\IIS Express\IisExpressAdminCmd.exe"

& $iisExpressAdminCmdExe setupSslUrl -url:https://localhost:$port/ -UseSelfSigned