#
# Module manifest for module 'virotYubikey'
#
# Generated by: Oscar Virot
#
# Generated on: 2024-05-25
#

@{

# Script module or binary module file associated with this manifest.
RootModule = 'powershellYK.dll'

# Version number of this module.
ModuleVersion = '0.0.0.0'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = 'd947dd9b-87eb-49ea-a373-b91c7acc0917'

# Author of this module
Author = 'Oscar Virot'

# Company or vendor of this module
CompanyName = ''

# Copyright statement for this module
Copyright = '(c) Oscar Virot. All rights reserved.'

# Description of the functionality provided by this module
Description = 'A unofficial powershell wrapper for Yubico .NET SDK. Allows administration of Yubikeys from Powershell.'

# Minimum version of the PowerShell engine required by this module
PowerShellVersion = '7.0'

# Name of the PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# DotNetFrameworkVersion = ''

# Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# ClrVersion = ''

# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = 'None'

# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @()

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @('powershellYK.format.ps1xml')

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
NestedModules = @()

# Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
# FunctionsToExport = @()

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = @(
 'Connect-Yubikey',
 'Disconnect-Yubikey',
 'Find-Yubikey',
 'Get-Yubikey',
 'Lock-Yubikey',
 'Set-Yubikey',
 'Unlock-Yubikey',
 'Connect-YubikeyFIDO2',
 'Enable-YubikeyFIDO2EnterpriseAttestation',
 'Get-YubikeyFIDO2',
 'Get-YubikeyFIDO2Credentials',
 'Set-YubikeyFIDO2',
 'Reset-YubikeyFIDO2',
 'Connect-YubikeyOATH',
 'Get-YubikeyOATH',
 'Get-YubikeyOATHAccount',
 'New-YubikeyOATHAccount',
 'Remove-YubikeyOATHAccount',
 'Protect-YubikeyOATH',
 'Reset-YubikeyOATH',
 'Request-YubikeyOATHCode',
 'Rename-YubikeyOATHAccount',
 'Unprotect-YubikeyOATH',
 'Connect-YubikeyOTP',
 'Get-YubikeyOTP',
 'Remove-YubikeyOTP',
 'Request-YubikeyOTPChallange',
 'Switch-YubikeyOTP',
 'Set-YubikeyOTP',
 'Assert-YubikeyPIV',
 'Block-YubikeyPIV',
 'Build-YubiKeyPIVCertificateSigningRequest',
 'Build-YubikeyPIVSignCertificate',
 'Connect-YubikeyPIV',
 'Export-YubikeyPIVCertificate',
 'Get-YubikeyPIV',
 'Import-YubikeyPIV',
 'Move-YubikeyPIV',
 'New-YubikeyPIVKey',
 'New-YubikeyPIVSelfSign',
 'Remove-YubikeyPIVKey',
 'Reset-YubikeyPIV',
 'Unblock-YubikeyPIV',
 'Set-YubikeyPIV',
 'Get-YubikeyUVFingerprint',
 'Register-YubikeyUVFingerprint',
 'Remove-YubikeyUVFingerprint',
 'Rename-YubikeyUVFingerprint',
 'Confirm-YubikeyAttestion',
 'ConvertTo-AltSecurity',
 'Enable-PowershellYKSDKLogging',
 'Get-PowershellYKInfo'
)

# Variables to export from this module
VariablesToExport = @()

# Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
AliasesToExport = @('New-YubikeyPIVCSR')

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        Tags = @('Yubikey','PIV','FIDO2','TOTP','OATH')

        # A URL to the license for this module.
        # LicenseUri = ''

        # A URL to the main website for this project.
        ProjectUri = 'https://github.com/virot/powershellYK'

        # A URL to an icon representing this module.
        # IconUri = ''

        # ReleaseNotes of this module
        # ReleaseNotes = ''

        # Prerelease string of this module
        # Prerelease = ''

        # Flag to indicate whether the module requires explicit user acceptance for install/update/save
        # RequireLicenseAcceptance = $false

        # External dependent modules of this module
        # ExternalModuleDependencies = @()

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

