Describe "Test Yubikey parts" {
    It "outputs 'Verify that Find-Yubikey throws fault, if serialnumber does not exist on present yubikeys.'" {
        {Find-Yubikey -Serialnumber 123} | Should -Throw # Try to connect to non existing Yubikey
    }
    It "outputs 'Verify that connect works'" {
        Get-Yubikey|Select-Object -ExpandProperty SerialNumber | Should -Be 19661687
    }
}
