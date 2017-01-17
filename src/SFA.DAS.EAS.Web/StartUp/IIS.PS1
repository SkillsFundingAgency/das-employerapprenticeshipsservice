#Function to get the location of the cert#
Function GetLocation {
$cmdOutput = $($Command = "StartUp\FindPrivateKey.exe"
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



#Static Variables#

$checkcert=(Get-ChildItem cert:\LocalMachine\My | Where thumbprint -eq $thumbprint).Thumbprint
$Ctx = New-AzureStorageContext -ConnectionString "$ConfigurationStorageConnectionString"
$BlobName = "$environmentname/DasIDPCert.pfx"
$ContainerName = "certs"
$localTargetDirectory = "C:\Cert"

[Reflection.Assembly]::LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime")
$ConfigurationStorageConnectionString = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("ConfigurationStorageConnectionString")
$EnvironmentName = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("EnvironmentName")
$Thumbprint = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("TokenCertificateThumbprint")

if ($checkcert)
{
Write-Warning -Message "Certificate already installed"
#Apply Permissions Just Incase#
permissions 

}
else{
#If Certificate doesnt exist do the below#


#Create Folder#
New-Item $localTargetDirectory -type directory -ErrorAction SilentlyContinue -WarningAction SilentlyContinue

#Download Certifacte#

$error.clear()
Try {Get-AzureStorageBlobContent -Blob $BlobName -Container $ContainerName -Destination $localTargetDirectory -Context $ctx -Force }
catch {
"There has been an error downloading the file"

break
}

#Gets PFX Password from table storage#
$TableName = "Configuration"
$table = Get-AzureStorageTable –Name $TableName -Context $Ctx -ErrorAction SilentlyContinue
$query = New-Object Microsoft.WindowsAzure.Storage.Table.TableQuery 

if (!$table)
{
write-host "Configuration table does not exist"
break
}

#Define columns to select.
$list = New-Object System.Collections.Generic.List[string]
$list.Add("PartitionKey")
$list.Add("RowKey")
$list.Add("Data")

$query.FilterString =  "RowKey eq 'SFA.DAS.EmployerUser.CertPassword' and PartitionKey eq '$EnvironmentName' "
$query.SelectColumns = $list

$entities = $table.CloudTable.ExecuteQuery($query)

$CertPassword=((($entities.Properties).Values).PropertyAsObject)

if (!$CertPassword)
{
write-host "Password not available"
break
}

$mypwd = ConvertTo-SecureString -String "$CertPassword" -Force –AsPlainText

Import-PfxCertificate –FilePath C:\Cert\$environmentname\DasIDPCert.pfx cert:\localMachine\my -Password $mypwd

#Setting Permission on the Cert for ReadAccess#

Permissions

#Removing the Certificate download location#

Remove-Item $localTargetDirectory -Force -Recurse

}
