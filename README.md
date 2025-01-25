# powershellYK

## ℹ️ About
**powershellYK** is a third-party PowerShell module built on the official YubiKey .NET SDK. It enables seamless management of YubiKeys directly from PowerShell _without_ requiring any additional Yubico tools or software installations.

## 📊 Feature coverage
For feature coverage as well as a comparison with other YubiKey management tools, please see this [page](./Docs/Feature_comparison.md).

## 💻 Prerequisites
_Use of the powershellYK module requires the following prerequisites be met:_
- PowerShell 7 (```pwsh```)

## 💾 Installation
_To install powershellYK:_

1. Open PowerShell
2. Execute command: ```Install-Module powershellYK```
3. Press ```Y``` when prompted to proceed with installation
4. Execute command: ```Import-Module powershellYK```

## 📖 Basic usage
_To get started with powershellYK:_

#### Enable command feedback
```powershell
$InformationPreference = 'Continue'
```
#### List available commands
```powershell
Get-Command -Module powershellYK
```
#### Learn about a specific command
```powershell
Get-Help <commandName>
```

**NOTE**: For Cmdlet instructions and examples please refer to [documentation](./Docs/Commands/powershellYK.md)
