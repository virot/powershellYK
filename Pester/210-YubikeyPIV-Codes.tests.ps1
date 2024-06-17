Describe "Test YubikeyPIV PIV/PUK management" -Tag "Destructive" {
    BeforeEach {
    }
    It "outputs 'Verify that connect works'" {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 3
    }
    It "outputs 'Test Set-YubikeyPIV -PINRetries 8 -PUKRetries 6'" {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
	{Set-YubikeyPIV -PinRetries 8 -PukRetries 6} | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 8
        Get-YubikeyPIV|Select -ExpandProperty PUKRetries | Should -Be 6
    }
    It "outputs 'Test Block-YubikeyPIV throws if not PIN nor PUK is selected'" {
        {Block-YubikeyPIV} | Should -Throw
    }
    It "outputs 'Test Block-YubikeyPIV -PIN'" {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
        {Block-YubikeyPIV -PIN } | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 3
        Get-YubikeyPIV|Select -ExpandProperty PinRetriesLeft | Should -Be 0
    }
    It "outputs 'Test Block-YubikeyPIV -PUK'" {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
        {Block-YubikeyPIV -PUK } | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PUKRetries | Should -Be 3
        Get-YubikeyPIV|Select -ExpandProperty PUKRetriesLeft | Should -Be 0
    }
    It "outputs 'Test Block-YubikeyPIV -PIN -PUK'" {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Not -Throw
	{Set-YubikeyPIV -PinRetries 4 -PukRetries 5} | Should -Not -Throw
        {Block-YubikeyPIV -PIN -PUK } | Should -Not -Throw
        Get-YubikeyPIV|Select -ExpandProperty PinRetries | Should -Be 4
        Get-YubikeyPIV|Select -ExpandProperty PUKRetries | Should -Be 5
        Get-YubikeyPIV|Select -ExpandProperty PinRetriesLeft | Should -Be 0
        Get-YubikeyPIV|Select -ExpandProperty PUKRetriesLeft | Should -Be 0
    }
    It "outputs 'Change PIN and Verify'" {
        {Reset-YubikeyPIV -Confirm:$false} | Should -Not -Throw
	{Set-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force)} | Should -Not -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)} | Should -Throw
        {Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "654321" -AsPlainText -Force)} | Should -Not -Throw
    }
}