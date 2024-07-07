BeforeAll -Scriptblock {
    Reset-YubikeyPIV -Confirm:$false
    Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)
}

Describe "Remove-YubikeyPIV" -Tag @("Remove-YubikeyPIV","Yubikey5.7")  {
    It -Name "Verify creation before removal" -Test {
	{Get-YubikeyPIV -Slot 0x9a} | Should -Throw
        {New-YubikeyPIVKey -Slot "PIV Authentication" -Algorithm Rsa1024 -PinPolicy Default -TouchPolicy Default -Confirm:$False} | Should -Not -Throw
	{Get-YubikeyPIV -Slot 0x9a} | Should -Not -Throw
    }
    It -Name "Verify removal" -Test {
        {NRemove-YubikeyPIVKey -Slot "PIV Authentication" -Confirm:$false} | Should -Not -Throw
	{Get-YubikeyPIV -Slot 0x9a} | Should -Throw
    }
}