if (Test-Path 'release') {
    Remove-Item -Recurse 'release'
}
$Directory = New-Item -Type Directory 'release'

dotnet publish module --nologo --framework 'net8.0' --output "$($Directory.fullname)"
#dotnet publish powershellYK_loader --nologo --framework 'net8.0' --output "$($Directory.fullname)\loader"

#Copy-Item "$($Directory.fullname)\loader\powershellYK_loader.dll" "$($Directory.fullname)\module"
#Copy-Item "$($Directory.fullname)\loader\powershellYK_loader.pdb" "$($Directory.fullname)\module"
#Remove-Item -Recurse "$($Directory.fullname)\loader"
#Move-Item "$($Directory.fullname)\module\powershellYK.psd1" "$($Directory.fullname)"
#Move-Item "$($Directory.fullname)\module\powershellYK.format.ps1xml" "$($Directory.fullname)"

#Remove-Item -Recurse "$($Directory.fullname)\module\runtimes\linux*"
#Remove-Item -Recurse "$($Directory.fullname)\module\runtimes\osx*"
#Remove-Item -Recurse "$($Directory.fullname)\module\runtimes\unix*"


& "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x86\signtool.exe" sign /sha1 "8079DD82969461B1B7A8769B26262726AA0F6D89" /fd SHA256 /t http://timestamp.sectigo.com "$($Directory.fullname)\powershellYK.format.ps1xml"
& "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x86\signtool.exe" sign /sha1 "8079DD82969461B1B7A8769B26262726AA0F6D89" /fd SHA256 /t http://timestamp.sectigo.com "$($Directory.fullname)\powershellYK.dll"

#Get-Item "$($Directory.fullname)\powershellYK.psd1" -PipelineVariable ItemFile |ForEach {(Get-Content $ItemFile).Replace('RootModule = ''powershellYK.dll''','RootModule = ''.\module\powershellYK.dll''', [System.StringComparison]::InvariantCultureIgnoreCase) | Set-Content -Path $ItemFile }
#Update-ModuleManifest -Path "$($Directory.fullname)\powershellYK.psd1" -ModuleVersion (GI .\release\module\powershellYK.dll).VersionInfo.FileVersion.toString()
Update-Metadata -Path "$($Directory.fullname)\powershellYK.psd1" -PropertyName ModuleVersion -Value (GI .\release\powershellYK.dll).VersionInfo.FileVersion.toString()
#Update-Metadata -Path "$($Directory.fullname)\powershellYK.psd1" -PropertyName NestedModules -Value ".\module\powershellYK_loader.dll"

Import-Module "$($Directory.fullname)\powershellYK.psd1"

$parameters = @{	
    Path = '.\Documentation\Commands'
    RefreshModulePage = $true
    AlphabeticParamsOrder = $true
    UpdateInputOutput = $true
    ExcludeDontShow = $true
    LogPath = '\temp\platyps.log'
    Encoding = [System.Text.Encoding]::UTF8
}
Update-MarkdownHelpModule @parameters

New-ExternalHelp -Path '.\Documentation\Commands' -OutputPath "$($Directory.fullname)" -Force



Import-Module Pester
$configuration = [PesterConfiguration]::Default
# adding properties & discover via intellisense
$configuration.Run.Path = '.'
$configuration.Filter.Tag = @('Dry')
$configuration.Filter.ExcludeTag = @('Destructive')
$configuration.Should.ErrorAction = 'Stop'
$configuration.CodeCoverage.Enabled = $False

#Invoke-Pester -Configuration $configuration

