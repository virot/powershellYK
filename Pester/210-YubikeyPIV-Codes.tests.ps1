Describe "Test YubikeyPIV PIV/PUK management" -Tag "Destructive" {
    BeforeEach {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        {Connect-YubikeyPIV} | Should -Not -Throw
    }
    It "outputs 'Verify that connect works'" {
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 3
    }
    It "outputs 'Test Block-YubikeyPIV throws if not PIN nor PUK is selected'" {
        {Block-YubikeyPIV} | Should -Throw
    }
    It "outputs 'Test Block-YubikeyPIV -PIN'" {
        {Block-YubikeyPIV -PIN } | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 3
    }
    It "outputs 'Test Block-YubikeyPIV -PUK'" {
        {Block-YubikeyPIV -PUK } | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PUKRetries | Should -Be 3
    }
    It "outputs 'Test Block-YubikeyPIV -PIN -PUK'" {
        {Block-YubikeyPIV -PIN -PUK } | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 3
        Get-YubikeyPIV|Select -ExpandProperty PUKRetries | Should -Be 3
    }
    It "outputs 'Test Set-YubikeyPIV -PINRetries 8 -PUKRetries 6'" {
	{Set-YubikeyPIV -PinRetries 8 -PukRetries 6} | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 8
        Get-YubikeyPIV|Select -ExpandProperty PUKRetries | Should -Be 6
    }

}
