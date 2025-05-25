Describe "Test YubikeyPIV PIV/PUK management" -Tag "PIV" {
    BeforeAll {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
	{New-YubikeyPIVKey -Slot "0x9c" -PinPolicy Default -Algorithm ECP256 -TouchPolicy Never} | Should -Not -Throw
	{New-YubikeyPIVSelfSign -Slot "0x9c"} | Should -Not -Throw
    }
    It "PIVSlot 'string byte' works" {
	{Get-YubikeyPIV -Slot "0x9c"} | Should -Not -Throw 
    }
    It "PIVSlot 'integer casted' works" {
	{Get-YubikeyPIV -Slot ([int]0x9c)} | Should -Not -Throw 
    }
    It "PIVSlot 'integer' works" {
	{Get-YubikeyPIV -Slot 0x9c} | Should -Not -Throw 
    }
    It "PIVSlot 'byte' works" {
	{Get-YubikeyPIV -Slot ([byte]0x9c)} | Should -Not -Throw 
    }
    It "PIVSlot 'string name' works" {
	{Get-YubikeyPIV -Slot "Digital Signature"} | Should -Not -Throw 
    }
}