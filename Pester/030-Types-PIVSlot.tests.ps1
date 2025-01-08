Describe "030 Test PIVSlot" -Tag "Without-YubiKey" {
    AfterEach -Scriptblock {
        Remove-Variable -Name Test -ErrorAction SilentlyContinue
    }
    It "Test create new slot without information" {
        {[powershellYK.PIV.PIVSlot]::new()} | Should -Throw # PIVSlot cannot be null
    }
    It "Test create new slot with incorrect information" {
        {[powershellYK.PIV.PIVSlot]::new(0x01)} | Should -Throw # PIVSlot must be a YubiKey PIV slot
        {[powershellYK.PIV.PIVSlot]::new(0xff)} | Should -Throw # PIVSlot must be a YubiKey PIV slot
        {[powershellYK.PIV.PIVSlot]::new(0x79)} | Should -Throw # PIVSlot must be a YubiKey PIV slot
    }
    It "Verify output type" {
        [powershellYK.PIV.PIVSlot]::new(0x9a) | Should -BeOfType 'powershellYK.PIV.PIVSlot'
    }
    It "Test create slot with [Integer]" {
        {[powershellYK.PIV.PIVSlot]::new([int]0x9a)} | Should -Not -Throw
        {[powershellYK.PIV.PIVSlot]::new([int]0xffff)} | Should -Throw
    }
    It "Test create slot with [Byte]" {
        {[powershellYK.PIV.PIVSlot]::new([Byte]0x9a)} | Should -Not -Throw
    }
    It "Test create slot with [String]" {
        {[powershellYK.PIV.PIVSlot]::new([string]"0x9a")} | Should -Not -Throw
        {[powershellYK.PIV.PIVSlot]::new([string]"0x9A")} | Should -Not -Throw
        {[powershellYK.PIV.PIVSlot]::new([string]"PIV Authentication")} | Should -Not -Throw
    }
    It "Test create slot with [Yubico.YubiKey.Piv.PivSlot]" {
        {[powershellYK.PIV.PIVSlot]::new([string]"0x9a")} | Should -Not -Throw
        {[powershellYK.PIV.PIVSlot]::new([string]"0x9A")} | Should -Not -Throw
        {[powershellYK.PIV.PIVSlot]::new([string]"PIV Authentication")} | Should -Not -Throw
    }
    It "Test ToString()" {
        {[powershellYK.PIV.PIVSlot]::new(0x9a).ToString()} | Should -Not -Throw
        [powershellYK.PIV.PIVSlot]::new(0x9a).ToString() | Should -Be "0x9A"
    }
    It "Test ToNamedSlot()" {
        [powershellYK.PIV.PIVSlot]::new(0x9a).ToNamedSlot() | Should -Be "PIV Authentication"
    }
    It "Test ToByte()" {
        [powershellYK.PIV.PIVSlot]::new(0x9a).ToByte() | Should -BeOfType 'Byte'
    }

#Verify Operators
    It "Test Explicit Cast to [Byte]" {
        {[byte][powershellYK.PIV.PIVSlot]::new(0x9a)} | Should -Not -Throw
    }
    It "Test Explicit Cast to [Int]" {
        {[int][powershellYK.PIV.PIVSlot]::new(0x9a)} | Should -Not -Throw
    }
}
