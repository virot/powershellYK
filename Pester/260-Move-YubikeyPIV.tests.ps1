BeforeAll -Scriptblock {
    Reset-YubikeyPIV -Confirm:$false
    Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)
}

Describe "Move-YubikeyPIV" -Tag @("Move-YubikeyPIV","Yubikey5.7")  {
    It -Name "Prework for move tests" -Test {
	{New-YubikeyPIVKey -Slot "PIV Authentication" -Algorithm Rsa1024 -PinPolicy Default -TouchPolicy Default} | Should -Not -Throw
        {New-YubikeyPIVSelfSign -Slot "PIV Authentication"} | Should -Not -Throw
        {Get-YubikeyPIV -Slot 0x9a} | Should -Not -Throw
    }
    It -Name "Verify move without certificate" -Test {
        {Get-YubikeyPIV -Slot 0x9d} | Should -Throw
	{Move-YubikeyPIV -SourceSlot "PIV Authentication" -DestinationSlot 0x9d} | Should -Not -Throw
        {Get-YubikeyPIV -Slot 0x9d} | Should -Not -Throw
        (Get-YubikeyPIV -Slot 0x9d).Certificate | Should -BeNullOrEmpty
    }

    It -Name "Prework for move tests with cert" -Test {
	{New-YubikeyPIVKey -Slot "PIV Authentication" -Algorithm Rsa1024 -PinPolicy Default -TouchPolicy Default -Confirm:$False} | Should -Not -Throw
        {New-YubikeyPIVSelfSign -Slot "PIV Authentication" -Confirm:$False} | Should -Not -Throw
        {Get-YubikeyPIV -Slot 0x9a} | Should -Not -Throw
    }
    It -Name "Verify move with certificate" -Test {
        {Get-YubikeyPIV -Slot 0x9c} | Should -Throw
	{Move-YubikeyPIV -SourceSlot "PIV Authentication" -DestinationSlot 0x9c -MigrateCertificate} | Should -Not -Throw
        {Get-YubikeyPIV -Slot 0x9c} | Should -Not -Throw
        (Get-YubikeyPIV -Slot 0x9c).Certificate | Should -BeOfType System.Security.Cryptography.X509Certificates.X509Certificate2
    }
}