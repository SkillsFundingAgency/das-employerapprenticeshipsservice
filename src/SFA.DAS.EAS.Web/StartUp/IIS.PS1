function Install-MSIFile {

	[CmdletBinding()]
	 Param(
	  [parameter(mandatory=$true,ValueFromPipeline=$true,ValueFromPipelinebyPropertyName=$true)]
			[ValidateNotNullorEmpty()]
			[string]$msiFile,

			[parameter()]
			[ValidateNotNullorEmpty()]
			[string]$targetDir
	 )

	#Logging in the first instance#
	Import-Module -Name "..\..\approot\bin\SFA.DAS.NLog.Targets.Redis.dll"
	Import-Module -Name "..\..\approot\bin\StackExchange.Redis.dll"
	Import-Module -Name "..\..\approot\bin\NLog.dll"
	Import-Module -Name "..\..\approot\bin\Newtonsoft.Json.dll"
	Import-Module -Name "..\..\approot\bin\Microsoft.WindowsAzure.Configuration.dll"
	Import-Module -Name "..\..\approot\bin\Microsoft.WindowsAzure.Storage.dll"

	#Redis Logging 
	$drive=(Get-Location).Drive.Root
	$config1="approot\web.config"
	$config= "$drive$config1"

	$fe = New-Object NLog.Config.XmlLoggingConfiguration("$config")

	([NLog.LogManager]::Configuration) = $fe
	$logger = [NLog.LogManager]::GetCurrentClassLogger()

		$logger.Error("Installing MSI")

	if (!(Test-Path $msiFile)){
		throw "Path to the MSI File $($msiFile) is invalid. Please supply a valid MSI file"
	}
	$arguments = @(
		"/i"
		"`"$msiFile`""
		"/qn"
	)
	if ($targetDir){
		if (!(Test-Path $targetDir)){
			throw "Path to the Installation Directory $($targetDir) is invalid. Please supply a valid installation directory"
		}
		$arguments += "INSTALLDIR=`"$targetDir`""
	}
	Write-Verbose "Installing $msiFile....."
	$process = Start-Process -FilePath msiexec.exe -ArgumentList $arguments -Wait -PassThru
	if ($process.ExitCode -eq 0){
		Write-Verbose "$msiFile has been successfully installed"
		$logger.Error("$msiFile has been successfully installed")
		Restart-Computer
	}
	else {
		Write-Verbose "installer exit code  $($process.ExitCode) for file  $($msifile)"
		$logger.Error("installer exit code  $($process.ExitCode) for file  $($msifile)")
	}
}
#Function to get the location of the cert#
Function GetLocation {
	$drive=(Get-Location).Drive.Root
	$config2="approot\Startup\FindPrivateKey.exe"
	$config3= "$drive$config2"
	$cmdOutput = $($Command = "$config3"
	$Parms = "My LocalMachine -t ""$thumbprint"" -a "
	$Prms = $Parms.Split(" ")
	& "$Command" $Prms
	return $cmdOutput
	)
}
#Set Permissions on the cert#
Function Permissions {
	$location=GetLocation
	$acl = Get-Acl "$location"
	$permission = "USERS","FullControl","Allow"
	$accessRule = new-object System.Security.AccessControl.FileSystemAccessRule $permission
	$acl.SetAccessRule($accessRule)
	$acl | Set-Acl "$location"
}


Import-Module -Name "..\..\approot\bin\SFA.DAS.NLog.Targets.Redis.dll"
Import-Module -Name "..\..\approot\bin\StackExchange.Redis.dll"
Import-Module -Name "..\..\approot\bin\NLog.dll"
Import-Module -Name "..\..\approot\bin\Newtonsoft.Json.dll"
Import-Module -Name "..\..\approot\bin\Microsoft.WindowsAzure.Configuration.dll"

#Redis Logging 
$drive=(Get-Location).Drive.Root
$config1="approot\web.config"
$config= "$drive$config1"

$fe = New-Object NLog.Config.XmlLoggingConfiguration("$config")
([NLog.LogManager]::Configuration) = $fe
$logger = [NLog.LogManager]::GetCurrentClassLogger()

$Azure="Azure"

if(Get-Module -ListAvailable | Where-Object{ $_.Name -eq $Azure }) 
{  

	[Reflection.Assembly]::LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime")
	$ConfigurationStorageConnectionString = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("ConfigurationStorageConnectionString")
	$EnvironmentName = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("EnvironmentName")
	$Thumbprint = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("TokenCertificateThumbprint")

	$checkcert=(Get-ChildItem cert:\LocalMachine\My | Where thumbprint -eq $thumbprint).Thumbprint
	$Ctx = New-AzureStorageContext -ConnectionString "$ConfigurationStorageConnectionString"
	$BlobName = "$environmentname/DasIDPCert.pfx"
	$BlobNamePassword = "$environmentname/password.txt"
	$ContainerName = "certs"
	$localTargetDirectory = "C:\Cert"
			 
	if ($checkcert)
	{
		Write-Warning -Message "Certificate already installed"
		#Apply Permissions Just Incase#
		permissions 
		$logger.Info("Certificate already Installed with thumbprint $Thumbprint")
	}
	else{
		#If Certificate doesnt exist do the below#

		#Create Folder#
		New-Item $localTargetDirectory -type directory -ErrorAction SilentlyContinue -WarningAction SilentlyContinue

		#Download Certifacte & Password#
		$error.clear()
		Try {Get-AzureStorageBlobContent -Blob $BlobName -Container $ContainerName -Destination $localTargetDirectory -Context $ctx -Force }
		catch {
			"There has been an error downloading the file"
			$logger.Error("There has been an error downloading the file")
			break
		}

		$error.clear()
		Try {Get-AzureStorageBlobContent -Blob $BlobNamePassword -Container $ContainerName -Destination $localTargetDirectory -Context $ctx -Force }
		catch {
			"There has been an error downloading the file"
			$logger.Error("There has been an error downloading the password file")
			break
		}

		$CertPassword = Get-Content "C:\Cert\$environmentname\password.txt"

		$mypwd = ConvertTo-SecureString -String "$CertPassword" -Force -AsPlainText

		Import-PfxCertificate -FilePath C:\Cert\$environmentname\DasIDPCert.pfx cert:\localMachine\my -Password $mypwd
		$logger.Info("Importing the certificate")
		#Setting Permission on the Cert for ReadAccess#
		Permissions
		$logger.Info("Setting Permissions on the certificate")
		#Removing the Certificate download location#

		Remove-Item $localTargetDirectory -Force -Recurse
		$logger.Info("Removing local folder")
	
	}

}
else  
{  
 "StartUp\Powershell.msi" | Install-MSIFile
}

