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
}
else {
    Write-Verbose "installer exit code  $($process.ExitCode) for file  $($msifile)"
}
}
$Azure="Azure"


if(Get-Module -ListAvailable | Where-Object{ $_.Name -eq $Azure }) 
{  
[Reflection.Assembly]::LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime")
$ConfigurationStorageConnectionString = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("ConfigurationStorageConnectionString")
$EnvironmentName = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("EnvironmentName")

#Query
$Ctx = New-AzureStorageContext -ConnectionString $ConfigurationStorageConnectionString
$BlobName = "$environmentname/DasIDPCert.pfx"
$ContainerName = "certs"
$localTargetDirectory = "C:\Cert"

New-Item $localTargetDirectory -type directory -ErrorAction SilentlyContinue -WarningAction SilentlyContinue
 
Get-AzureStorageBlobContent -Blob $BlobName -Container $ContainerName -Destination $localTargetDirectory -Context $ctx -Force 

$TableName = "Configuration"

$table = Get-AzureStorageTable –Name $TableName -Context $Ctx
$query = New-Object Microsoft.WindowsAzure.Storage.Table.TableQuery

#Define columns to select.
$list = New-Object System.Collections.Generic.List[string]
$list.Add("PartitionKey")
$list.Add("RowKey")
$list.Add("Data")

$query.FilterString =  "RowKey eq 'SFA.DAS.EmployerUser.CertPassword' and PartitionKey eq '$EnvironmentName' "
$query.SelectColumns = $list

$entities = $table.CloudTable.ExecuteQuery($query)

$password=$entities.Properties
$password1=$password.Values
$CertPassword=$password1.PropertyAsObject

$mypwd = ConvertTo-SecureString -String "$CertPassword" -Force –AsPlainText

Import-PfxCertificate –FilePath C:\Cert\$environmentname\DasIDPCert.pfx cert:\localMachine\my -Password $mypwd

#Setting Permission on the Cert for ReadAccess#
	Start-sleep -s 15

$Thumbprint=Get-ChildItem -path cert:\LocalMachine\My | where subject -eq "CN=idsrv3test"
$Thumb=$thumbprint.Thumbprint

$cmdOutput = $($Command = "StartUp\FindPrivateKey.exe"
$Parms = "My LocalMachine -t ""$thumb"" -a "
$Prms = $Parms.Split(" ")
& "$Command" $Prms
)
	
$acl = Get-Acl $cmdOutput
$permission = "USERS","FullControl","Allow"
$accessRule = new-object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $cmdOutput

}  
else  
{  
 "StartUp\Powershell.msi" | Install-MSIFile
}

