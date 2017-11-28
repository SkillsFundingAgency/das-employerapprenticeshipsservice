#Requires -RunAsAdministrator

$certpwd = ConvertTo-SecureString -String password -Force -AsPlainText

Import-PfxCertificate -FilePath $PSScriptRoot\localhost.pfx -CertStoreLocation cert://LocalMachine/My -Password $certpwd -Exportable
Import-PfxCertificate -FilePath $PSScriptRoot\localhost.pfx -CertStoreLocation cert://LocalMachine/Root -Password $certpwd -Exportable
Import-PfxCertificate -FilePath $PSScriptRoot\DasAmlCert.pfx -CertStoreLocation cert://LocalMachine/My -Password $certpwd -Exportable
Import-PfxCertificate -FilePath $PSScriptRoot\DasAmlCert.pfx -CertStoreLocation cert://LocalMachine/Root -Password $certpwd -Exportable

$idppwd = ConvertTo-SecureString -String idsrv3test -Force -AsPlainText

Import-PfxCertificate -FilePath $PSScriptRoot\DasIDPCert.pfx -CertStoreLocation cert://LocalMachine/My -Password $idppwd -Exportable