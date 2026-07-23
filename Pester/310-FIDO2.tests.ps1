Describe "FIDO2 Tests" -Tag @("FIDO2")  {
    BeforeAll {
        Connect-YubiKey
        Connect-YubiKeyFIDO2 -PIN (ConvertTo-SecureString -String '123456' -AsPlainText -Force)
        Get-YubiKeyFIDO2Credential| ForEach { Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false} 
    }
    It -Name "Verify connect to FIDO2" -Test {
        {Connect-YubikeyFIDO2 -PIN (ConvertTo-SecureString -String "612345" -AsPlainText -Force)} | Should -Throw
    }
    It -Name "Change PIN on FIDO2" -Test {
        {Set-YubikeyFIDO2PIN -OldPIN (ConvertTo-SecureString -String "612345" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString -String "612345" -AsPlainText -Force)} | Should -Throw
        {Set-YubikeyFIDO2PIN -OldPIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force)} | Should -Not -Throw
        {Connect-YubikeyFIDO2 -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Throw
        {Connect-YubikeyFIDO2 -PIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force)} | Should -Not -Throw
        {Set-YubikeyFIDO2PIN -OldPIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
    }
    It -Name "Create synthetic credential (no IdP)" -Test {
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK-synthetic' -Username 'syntheticUser'} | Should -Not -Throw
        (Get-YubiKeyFIDO2Credential | Where-Object { $_.RPId -eq 'powershellYK-synthetic' }).UserName | Should -Be 'syntheticUser'
        {Get-YubiKeyFIDO2Credential | Where-Object { $_.RPId -eq 'powershellYK-synthetic' } | ForEach-Object { Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false }} | Should -Not -Throw
    }
    It -Name "Create synthetic credential with display name" -Test {
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK-synthetic2' -Username 'synUser2' -UserDisplayName 'Synthetic User Two'} | Should -Not -Throw
        {Get-YubiKeyFIDO2Credential | Where-Object { $_.RPId -eq 'powershellYK-synthetic2' } | ForEach-Object { Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false }} | Should -Not -Throw
    }
    It -Name "Clear all credentials" -Test {
        {Get-YubiKeyFIDO2Credential|%{Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false}} | Should -Not -Throw
	Get-YubiKeyFIDO2Credential -WarningAction SilentlyContinue | Should -BeNullOrEmpty
	#[array](Get-YubiKeyFIDO2Credential -WarningAction SilentlyContinue)).Count | Should -Be 0  # Make sure that the warning message does not trip Pester
    }
    It -Name "Create single fake credential" -Test {
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYKUser' -UserID 0x01} | Should -Not -Throw
	Get-YubiKeyFIDO2Credential | Should -HaveCount 1
	(Get-YubiKeyFIDO2Credential).UserName | Should -Be "powershellYKUser"
	(Get-YubiKeyFIDO2Credential).RPId | Should -Be "powershellYK"
    }
    It -Name "Add another credential" -Test {
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYK' -UserID 0x02} | Should -Not -Throw
	(Get-YubiKeyFIDO2Credential).Count | Should -Be 2
    }
    It -Name "Clear all credentials one after another total 2" -Test {
        & {Get-YubiKeyFIDO2Credential|%{Remove-YubikeyFIDO2Credential -CredentialId $_.CredentialID -Confirm:$false}} 
	Get-YubiKeyFIDO2Credential -WarningAction SilentlyContinue | Should-BeFalsy
    }

    It -Name "Create credential and remove using CredentialID" -Test {
        {New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYK' -UserID 0x03} | Should -Not -Throw
        $credentialString = (Get-YubiKeyFIDO2Credential|Select-Object -ExpandProperty CredentialID).ToString()
        {Remove-YubikeyFIDO2Credential -CredentialId $credentialString -Confirm:$false} | Should -Not -Throw
    }
}