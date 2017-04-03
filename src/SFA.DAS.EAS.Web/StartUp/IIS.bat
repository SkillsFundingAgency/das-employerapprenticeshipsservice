
PowerShell.exe -NonInteractive -ExecutionPolicy Unrestricted "StartUp\IIS.ps1" >> log.txt
PowerShell.exe -NonInteractive -ExecutionPolicy Unrestricted "StartUp\TokenServiceCertInstall.PS1" >> log2.txt

EXIT /B 0