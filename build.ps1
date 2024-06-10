if (Test-Path 'release') {
    Remove-Item -Recurse 'release'
}
$Directory = New-Item -Type Directory 'release'
dotnet publish module --nologo --framework 'net8.0' --output "$($Directory.fullname)\module"
dotnet publish powershellYK_loader --nologo --framework 'net8.0' --output "$($Directory.fullname)\loader"

Copy-Item "$($Directory.fullname)\loader\powershellYK_loader.dll" "$($Directory.fullname)\module"
Copy-Item "$($Directory.fullname)\loader\powershellYK_loader.pdb" "$($Directory.fullname)\module"
Remove-Item -Recurse "$($Directory.fullname)\loader"
Move-Item "$($Directory.fullname)\module\powershellYK.psd1" "$($Directory.fullname)"
Move-Item "$($Directory.fullname)\module\powershellYK.format.ps1xml" "$($Directory.fullname)"


& "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x86\signtool.exe" sign /sha1 "8079DD82969461B1B7A8769B26262726AA0F6D89" /fd SHA256 /t http://timestamp.sectigo.com "$($Directory.fullname)\powershellYK.format.ps1xml"
& "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x86\signtool.exe" sign /sha1 "8079DD82969461B1B7A8769B26262726AA0F6D89" /fd SHA256 /t http://timestamp.sectigo.com "$($Directory.fullname)\module\powershellYK.dll"

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

