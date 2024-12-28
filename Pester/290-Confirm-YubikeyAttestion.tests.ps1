BeforeAll -Scriptblock {
}

Describe "Confirm-YubikeyAttestion requestWithBuiltinAttestion" -Tag "Without-YubiKey" {
    BeforeEach -Scriptblock {
        #Get-Variable pest*|Remove-Variable
    }

    It -Name "Verify 'Confirm-YubikeyAttestion -CertificateRequest _fullfile_' works, Windows" -Skip:(!$IsWindows) -Test {
        $pest_return = Confirm-YubikeyAttestion -CertificateRequest "$PSScriptRoot\TestData\piv_attestion_certificaterequest_with_attestion.req"
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.AttestionValidated | Should -Be $True
        $pest_return.AttestionMatchesCSR | Should -Be $True
    }

    It -Name "Verify 'Confirm-YubikeyAttestion -CertificateRequest _fullfile_' works, Non-Windows" -Skip:($IsWindows) -Test {
        $pest_return = Confirm-YubikeyAttestion -CertificateRequest "$PSScriptRoot/TestData/piv_attestion_certificaterequest_with_attestion.req"
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.AttestionValidated | Should -Be $True
        $pest_return.AttestionMatchesCSR | Should -Be $True
    }

    It -Name "Verify 'Confirm-YubikeyAttestion -CertificateRequest _shortfile_' works, Windows" -Skip:(!$IsWindows) -Test {
        $pest_return = Confirm-YubikeyAttestion -CertificateRequest ".\Pester\TestData\piv_attestion_certificaterequest_with_attestion.req"
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.AttestionValidated | Should -Be $True
        $pest_return.AttestionMatchesCSR | Should -Be $True
    }

    It -Name "Verify 'Confirm-YubikeyAttestion -CertificateRequest _shortfile_' works, Non-Windows" -Skip:($IsWindows) -Test {
        $pest_return = Confirm-YubikeyAttestion -CertificateRequest "./Pester/TestData/piv_attestion_certificaterequest_with_attestion.req"
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.AttestionValidated | Should -Be $True
        $pest_return.AttestionMatchesCSR | Should -Be $True
    }

    It -Name "Verify 'Confirm-YubikeyAttestion -CertificateRequest _PEM_' works" -Test {
        [string]$pest_csr = (Get-Content "$PSScriptRoot\TestData\piv_attestion_certificaterequest_with_attestion.req")
        $pest_return = Confirm-YubikeyAttestion -CertificateRequest $pest_csr
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.AttestionValidated | Should -Be $True
        $pest_return.AttestionMatchesCSR | Should -Be $True
    }
    It -Name "Verify 'Confirm-YubikeyAttestion -CertificateRequest _CertificateRequest_' works" -Test {
#        [string[]]$pest_csr = (Get-Content "$PSScriptRoot\TestData\piv_attestion_certificaterequest_with_attestion.req")
#        [string]$pest_csr = ($pest_csr |?{$_ -notmatch '^-----.*-----$'}) -Join ''
#        $pest_input = [system.Text.Encoding]::UTF8.GetBytes($pest_csr)
$pest_input = [byte[]](0x30, 0x82, 0x6, 0xEC, 0x30, 0x82, 0x6, 0x72, 0x2, 0x1, 0x0, 0x30, 0x3E, 0x31, 0xD, 0x30, 0xB, 0x6, 0x3, 0x55, 0x4, 0xA, 0x13, 0x4, 0x46, 0x61, 0x6B, 0x65, 0x31, 0x2D, 0x30, 0x2B, 0x6, 0x3, 0x55, 0x4, 0x3, 0x13, 0x24, 0x53, 0x75, 0x62, 0x6A, 0x65, 0x63, 0x74, 0x4E, 0x61, 0x6D, 0x65, 0x20, 0x74, 0x6F, 0x20, 0x62, 0x65, 0x20, 0x73, 0x75, 0x70, 0x70, 0x6C, 0x69, 0x65, 0x64, 0x20, 0x62, 0x79, 0x20, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x30, 0x76, 0x30, 0x10, 0x6, 0x7, 0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x2, 0x1, 0x6, 0x5, 0x2B, 0x81, 0x4, 0x0, 0x22, 0x3, 0x62, 0x0, 0x4, 0x18, 0x49, 0xC5, 0x3, 0xBD, 0x4B, 0x4D, 0x42, 0x88, 0x2, 0xC5, 0x5F, 0xFB, 0x6C, 0xD4, 0x6A, 0x82, 0x8, 0xAE, 0xDE, 0xB7, 0x3E, 0x4E, 0xF3, 0x3D, 0x3, 0x1D, 0x1C, 0xE6, 0x3E, 0x39, 0x41, 0x42, 0x97, 0x4E, 0x67, 0x4A, 0x3C, 0x1F, 0x27, 0x0, 0x7D, 0xBA, 0x48, 0x58, 0x67, 0x40, 0x4A, 0xA7, 0x93, 0x3C, 0xB4, 0x2E, 0x2E, 0xE0, 0x4, 0xD1, 0xF5, 0x77, 0xCB, 0x3E, 0xD3, 0x6A, 0xC, 0x1C, 0xCD, 0x58, 0x8B, 0xD7, 0xA5, 0xBE, 0x70, 0x53, 0xBF, 0xA0, 0xB5, 0x4F, 0xD9, 0xCE, 0x2E, 0xF8, 0xAC, 0x11, 0xB9, 0x9C, 0x25, 0x33, 0x17, 0xB, 0x4E, 0x8A, 0x82, 0x1C, 0xCF, 0x1F, 0x52, 0xA0, 0x82, 0x5, 0xB3, 0x30, 0x82, 0x5, 0xAF, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x9, 0xE, 0x31, 0x82, 0x5, 0xA0, 0x30, 0x82, 0x5, 0x9C, 0x30, 0x82, 0x2, 0x86, 0x6, 0xA, 0x2B, 0x6, 0x1, 0x4, 0x1, 0x82, 0xC4, 0xA, 0x3, 0xB, 0x4, 0x82, 0x2, 0x76, 0x30, 0x82, 0x2, 0x72, 0x30, 0x82, 0x1, 0x5A, 0xA0, 0x3, 0x2, 0x1, 0x2, 0x2, 0x10, 0x1, 0x59, 0xCA, 0xD1, 0x15, 0x62, 0xB6, 0x6D, 0xBB, 0x89, 0xFE, 0x58, 0x2E, 0x77, 0x3D, 0xBC, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0xB, 0x5, 0x0, 0x30, 0x21, 0x31, 0x1F, 0x30, 0x1D, 0x6, 0x3, 0x55, 0x4, 0x3, 0xC, 0x16, 0x59, 0x75, 0x62, 0x69, 0x63, 0x6F, 0x20, 0x50, 0x49, 0x56, 0x20, 0x41, 0x74, 0x74, 0x65, 0x73, 0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x30, 0x20, 0x17, 0xD, 0x31, 0x36, 0x30, 0x33, 0x31, 0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x18, 0xF, 0x32, 0x30, 0x35, 0x32, 0x30, 0x34, 0x31, 0x37, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x30, 0x25, 0x31, 0x23, 0x30, 0x21, 0x6, 0x3, 0x55, 0x4, 0x3, 0xC, 0x1A, 0x59, 0x75, 0x62, 0x69, 0x4B, 0x65, 0x79, 0x20, 0x50, 0x49, 0x56, 0x20, 0x41, 0x74, 0x74, 0x65, 0x73, 0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x39, 0x61, 0x30, 0x76, 0x30, 0x10, 0x6, 0x7, 0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x2, 0x1, 0x6, 0x5, 0x2B, 0x81, 0x4, 0x0, 0x22, 0x3, 0x62, 0x0, 0x4, 0x18, 0x49, 0xC5, 0x3, 0xBD, 0x4B, 0x4D, 0x42, 0x88, 0x2, 0xC5, 0x5F, 0xFB, 0x6C, 0xD4, 0x6A, 0x82, 0x8, 0xAE, 0xDE, 0xB7, 0x3E, 0x4E, 0xF3, 0x3D, 0x3, 0x1D, 0x1C, 0xE6, 0x3E, 0x39, 0x41, 0x42, 0x97, 0x4E, 0x67, 0x4A, 0x3C, 0x1F, 0x27, 0x0, 0x7D, 0xBA, 0x48, 0x58, 0x67, 0x40, 0x4A, 0xA7, 0x93, 0x3C, 0xB4, 0x2E, 0x2E, 0xE0, 0x4, 0xD1, 0xF5, 0x77, 0xCB, 0x3E, 0xD3, 0x6A, 0xC, 0x1C, 0xCD, 0x58, 0x8B, 0xD7, 0xA5, 0xBE, 0x70, 0x53, 0xBF, 0xA0, 0xB5, 0x4F, 0xD9, 0xCE, 0x2E, 0xF8, 0xAC, 0x11, 0xB9, 0x9C, 0x25, 0x33, 0x17, 0xB, 0x4E, 0x8A, 0x82, 0x1C, 0xCF, 0x1F, 0x52, 0xA3, 0x4E, 0x30, 0x4C, 0x30, 0x11, 0x6, 0xA, 0x2B, 0x6, 0x1, 0x4, 0x1, 0x82, 0xC4, 0xA, 0x3, 0x3, 0x4, 0x3, 0x5, 0x4, 0x3, 0x30, 0x14, 0x6, 0xA, 0x2B, 0x6, 0x1, 0x4, 0x1, 0x82, 0xC4, 0xA, 0x3, 0x7, 0x4, 0x6, 0x2, 0x4, 0x1, 0x2C, 0x3, 0x77, 0x30, 0x10, 0x6, 0xA, 0x2B, 0x6, 0x1, 0x4, 0x1, 0x82, 0xC4, 0xA, 0x3, 0x8, 0x4, 0x2, 0x2, 0x1, 0x30, 0xF, 0x6, 0xA, 0x2B, 0x6, 0x1, 0x4, 0x1, 0x82, 0xC4, 0xA, 0x3, 0x9, 0x4, 0x1, 0x1, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0xB, 0x5, 0x0, 0x3, 0x82, 0x1, 0x1, 0x0, 0x1, 0x50, 0xF7, 0xAC, 0x9E, 0x6F, 0x6A, 0x3, 0x3B, 0xC0, 0x88, 0xA7, 0xB4, 0xC9, 0x3E, 0xB5, 0xB2, 0xAB, 0x57, 0x1B, 0x16, 0x6, 0x3A, 0x54, 0x99, 0x74, 0xC3, 0xC2, 0x18, 0xE2, 0x3A, 0x25, 0xB, 0x5D, 0x8E, 0x52, 0x7B, 0xD9, 0xA4, 0x7A, 0x4D, 0x98, 0x42, 0x85, 0x8C, 0x75, 0x77, 0xEC, 0xD4, 0x3A, 0x2E, 0xB3, 0x32, 0xFA, 0xE4, 0x42, 0x4E, 0x4E, 0xCD, 0xDD, 0x5A, 0x99, 0xCA, 0xF3, 0x8E, 0xBB, 0xA7, 0x84, 0xB9, 0xF1, 0x72, 0x5B, 0xAE, 0x97, 0x25, 0xF0, 0x9F, 0x5, 0x38, 0xE3, 0x5B, 0xAC, 0xE, 0xE7, 0x1, 0x7E, 0x76, 0xC7, 0x41, 0xF, 0x8E, 0xEB, 0x65, 0xE9, 0xEF, 0x86, 0x96, 0xD5, 0x61, 0x97, 0xE4, 0xDA, 0xE8, 0x90, 0xC1, 0x86, 0x89, 0x33, 0x5E, 0xA2, 0xDA, 0xA3, 0xD, 0x88, 0xFD, 0xCA, 0xD, 0x7A, 0xD6, 0x2D, 0x3D, 0xA0, 0x76, 0xAA, 0xC3, 0xCC, 0xEE, 0x7, 0xE2, 0xFE, 0x78, 0x94, 0xF2, 0xCE, 0x72, 0x6F, 0xF2, 0x39, 0x13, 0x53, 0xBC, 0xE9, 0x56, 0xEC, 0x29, 0x8, 0x1D, 0x23, 0x6F, 0xAB, 0x6C, 0x41, 0xF2, 0x53, 0x82, 0xCF, 0x35, 0xA2, 0xFD, 0x52, 0xFF, 0xED, 0xBC, 0x5F, 0x8B, 0xE1, 0x7E, 0x99, 0xDC, 0xB1, 0x84, 0x6C, 0x77, 0x49, 0xE, 0x10, 0xF4, 0x9F, 0x25, 0x86, 0x79, 0x4B, 0xC4, 0xD7, 0xF0, 0xFA, 0x39, 0x68, 0xB8, 0xD3, 0x1D, 0x84, 0x64, 0xFB, 0x56, 0x57, 0x1E, 0xD8, 0x2E, 0x51, 0xD1, 0x99, 0x72, 0xC2, 0x2D, 0x10, 0x4A, 0x18, 0xE0, 0xB, 0x9, 0x73, 0xCD, 0xF0, 0xD8, 0x3A, 0x65, 0xCA, 0x92, 0x8, 0x94, 0x93, 0xEB, 0x2B, 0xD4, 0x17, 0xAF, 0xB9, 0xC6, 0x62, 0x22, 0xCB, 0xEB, 0x28, 0xEE, 0xB1, 0x98, 0x66, 0x22, 0x86, 0x8, 0x16, 0x93, 0x8, 0x12, 0xA4, 0x12, 0x1E, 0xA0, 0x3E, 0x1B, 0x7C, 0x76, 0x60, 0x4F, 0x13, 0x30, 0x82, 0x3, 0xE, 0x6, 0xA, 0x2B, 0x6, 0x1, 0x4, 0x1, 0x82, 0xC4, 0xA, 0x3, 0x2, 0x4, 0x82, 0x2, 0xFE, 0x30, 0x82, 0x2, 0xFA, 0x30, 0x82, 0x1, 0xE2, 0xA0, 0x3, 0x2, 0x1, 0x2, 0x2, 0x9, 0x0, 0xE8, 0xC3, 0xDD, 0x79, 0x9E, 0x43, 0x3B, 0x62, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0xB, 0x5, 0x0, 0x30, 0x2B, 0x31, 0x29, 0x30, 0x27, 0x6, 0x3, 0x55, 0x4, 0x3, 0xC, 0x20, 0x59, 0x75, 0x62, 0x69, 0x63, 0x6F, 0x20, 0x50, 0x49, 0x56, 0x20, 0x52, 0x6F, 0x6F, 0x74, 0x20, 0x43, 0x41, 0x20, 0x53, 0x65, 0x72, 0x69, 0x61, 0x6C, 0x20, 0x32, 0x36, 0x33, 0x37, 0x35, 0x31, 0x30, 0x20, 0x17, 0xD, 0x31, 0x36, 0x30, 0x33, 0x31, 0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x18, 0xF, 0x32, 0x30, 0x35, 0x32, 0x30, 0x34, 0x31, 0x37, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x30, 0x21, 0x31, 0x1F, 0x30, 0x1D, 0x6, 0x3, 0x55, 0x4, 0x3, 0xC, 0x16, 0x59, 0x75, 0x62, 0x69, 0x63, 0x6F, 0x20, 0x50, 0x49, 0x56, 0x20, 0x41, 0x74, 0x74, 0x65, 0x73, 0x74, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x30, 0x82, 0x1, 0x22, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0x1, 0x5, 0x0, 0x3, 0x82, 0x1, 0xF, 0x0, 0x30, 0x82, 0x1, 0xA, 0x2, 0x82, 0x1, 0x1, 0x0, 0xBD, 0x3D, 0x3E, 0x27, 0xF4, 0x11, 0xEA, 0xCA, 0x9C, 0x15, 0x52, 0x8D, 0xAA, 0xBC, 0xEC, 0x90, 0x50, 0x6F, 0x7A, 0x9D, 0x96, 0x69, 0x2, 0xB2, 0x5F, 0x81, 0xCF, 0xED, 0x2, 0xCF, 0x3F, 0x62, 0x59, 0x36, 0x6E, 0xB3, 0x10, 0x8C, 0x72, 0x22, 0xD0, 0x1F, 0x1F, 0x68, 0x84, 0x5B, 0x8E, 0xB4, 0xAC, 0x39, 0x2E, 0xAF, 0x56, 0x4E, 0x99, 0x5F, 0xE3, 0xFF, 0x71, 0x7D, 0x6D, 0x47, 0x56, 0xB6, 0x84, 0xA6, 0x7E, 0x85, 0xB6, 0x94, 0xB8, 0x2E, 0xB0, 0x11, 0x33, 0xD0, 0x58, 0xE, 0x4B, 0xCD, 0x69, 0xBF, 0x3E, 0xA1, 0x71, 0xFD, 0xBE, 0x99, 0x6F, 0xA, 0x7A, 0x7, 0x6F, 0x66, 0xA0, 0xDB, 0x44, 0x8D, 0x8E, 0x39, 0x77, 0x77, 0xB6, 0xD7, 0xB4, 0xF, 0x8F, 0x9, 0x6B, 0xC0, 0xF8, 0xC7, 0x4E, 0x69, 0x8F, 0x67, 0xCC, 0x59, 0x87, 0xBC, 0x7F, 0x96, 0x2A, 0xDC, 0x18, 0xEF, 0x9E, 0xCE, 0x40, 0xB6, 0x55, 0x72, 0x43, 0x95, 0x1, 0x25, 0xB, 0x34, 0xCC, 0x55, 0x81, 0x35, 0xD0, 0xCF, 0x3D, 0x36, 0x27, 0x26, 0xDC, 0x36, 0x43, 0x86, 0x67, 0xF2, 0x96, 0x4C, 0x1C, 0x84, 0x57, 0xE0, 0x54, 0xCB, 0xA5, 0x7F, 0x35, 0x18, 0xD, 0xA1, 0xA7, 0x91, 0xE9, 0x68, 0x22, 0xD8, 0x65, 0x5E, 0x93, 0xF8, 0x5D, 0x53, 0x60, 0xCB, 0xAE, 0x4D, 0xC6, 0x7, 0x62, 0xCB, 0x8F, 0x1C, 0xAF, 0xCA, 0x13, 0x2E, 0xBD, 0x55, 0x2E, 0x14, 0xB7, 0x6E, 0x68, 0xD3, 0x67, 0x4E, 0xA5, 0x8F, 0x5D, 0xD2, 0x60, 0x23, 0x85, 0xED, 0xFB, 0xC0, 0x90, 0xBC, 0x9E, 0x99, 0xFC, 0x2E, 0x3F, 0x8F, 0x36, 0xD0, 0x6F, 0x8E, 0xFD, 0xAD, 0xEC, 0xD4, 0xF5, 0xD9, 0xA4, 0xBC, 0x5C, 0x6E, 0xC9, 0xF, 0xD5, 0x8B, 0x1A, 0x3A, 0xBE, 0xBE, 0x16, 0xF, 0x8, 0x4A, 0xDD, 0x1D, 0xE4, 0xC4, 0xDC, 0x27, 0xAE, 0xF7, 0x2, 0x3, 0x1, 0x0, 0x1, 0xA3, 0x29, 0x30, 0x27, 0x30, 0x11, 0x6, 0xA, 0x2B, 0x6, 0x1, 0x4, 0x1, 0x82, 0xC4, 0xA, 0x3, 0x3, 0x4, 0x3, 0x5, 0x4, 0x3, 0x30, 0x12, 0x6, 0x3, 0x55, 0x1D, 0x13, 0x1, 0x1, 0xFF, 0x4, 0x8, 0x30, 0x6, 0x1, 0x1, 0xFF, 0x2, 0x1, 0x0, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0xB, 0x5, 0x0, 0x3, 0x82, 0x1, 0x1, 0x0, 0x5B, 0x86, 0x79, 0x3D, 0x1D, 0x9A, 0x8D, 0xB5, 0x27, 0xAA, 0x81, 0xF5, 0x5F, 0xC3, 0x26, 0x1C, 0x26, 0x67, 0x5D, 0xFB, 0x0, 0xF4, 0xCD, 0x4, 0xAB, 0x34, 0x11, 0xF8, 0x57, 0xD2, 0x4A, 0xB7, 0x7F, 0xD8, 0xDF, 0xBB, 0xE5, 0x18, 0xB3, 0x6F, 0x61, 0x6B, 0x6, 0x87, 0x3F, 0xBE, 0xF7, 0x2A, 0xDB, 0x16, 0x39, 0x48, 0xD, 0xA1, 0x42, 0xBA, 0xB6, 0x9B, 0xEC, 0x36, 0x3B, 0xBE, 0x3, 0xF5, 0x1B, 0x1, 0xF5, 0x27, 0x2D, 0x45, 0x7D, 0xCE, 0xB8, 0x6C, 0x55, 0x2A, 0x4, 0x14, 0xD1, 0x2D, 0x32, 0x1A, 0xA3, 0x17, 0x5E, 0x95, 0x52, 0x35, 0x3E, 0x76, 0x47, 0x20, 0x8B, 0xF3, 0xCE, 0x3A, 0x8E, 0x6E, 0x34, 0x3B, 0x79, 0xEE, 0x7B, 0x83, 0xF6, 0x26, 0xD6, 0x19, 0xE9, 0x34, 0x7B, 0x8D, 0x65, 0x18, 0xA6, 0xE5, 0x45, 0x25, 0x99, 0xEA, 0x86, 0xB5, 0xD5, 0xB0, 0xB4, 0x3E, 0xE, 0x24, 0xC4, 0xE3, 0xA6, 0xC, 0xC7, 0xAD, 0x66, 0xF5, 0xA, 0xA1, 0x9E, 0xD0, 0x84, 0xDD, 0x98, 0xD3, 0x5D, 0xBD, 0xBF, 0x76, 0xC2, 0x2B, 0x7C, 0xDE, 0xFF, 0x7, 0xA1, 0xDF, 0xF1, 0xF7, 0x84, 0x74, 0xD1, 0x68, 0xE6, 0x7E, 0x1C, 0xA, 0x3E, 0x2C, 0x17, 0x9B, 0xF3, 0x7E, 0x50, 0x6C, 0x6B, 0xCA, 0x5E, 0xBC, 0x71, 0xE5, 0x2C, 0xC3, 0x6C, 0xBD, 0x59, 0x4C, 0x22, 0xC7, 0x14, 0x8C, 0x47, 0xCB, 0x90, 0x73, 0x29, 0xBA, 0x6B, 0x5D, 0xEA, 0x9A, 0xDB, 0x70, 0xCC, 0x69, 0xCB, 0x26, 0xB1, 0x35, 0x75, 0xBE, 0x65, 0x63, 0xB1, 0x8F, 0xE5, 0xE3, 0x69, 0x93, 0x25, 0xAC, 0x9A, 0xBD, 0x1, 0x2B, 0x1B, 0xDD, 0x7, 0xEE, 0x4, 0xC8, 0x98, 0x20, 0xD8, 0xC1, 0xC5, 0x4F, 0x1, 0xAD, 0xA7, 0x50, 0x58, 0xB8, 0xD9, 0xCB, 0xEC, 0x6D, 0xAC, 0x14, 0x9D, 0x3, 0x35, 0x22, 0x23, 0xF6, 0x98, 0x3F, 0x30, 0xA, 0x6, 0x8, 0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x4, 0x3, 0x3, 0x3, 0x68, 0x0, 0x30, 0x65, 0x2, 0x30, 0x76, 0x88, 0x93, 0xC4, 0x75, 0x59, 0xF9, 0xA2, 0x23, 0xB, 0xB, 0x3E, 0x1C, 0x2C, 0xCC, 0x8A, 0xF3, 0xAA, 0x84, 0x74, 0x6D, 0x80, 0xEE, 0x8A, 0xBF, 0xBF, 0xAD, 0x89, 0xEF, 0x92, 0x4C, 0xFB, 0xB, 0xCA, 0xF2, 0xBF, 0xB2, 0x5E, 0x2D, 0x80, 0xF8, 0x37, 0x8A, 0x6D, 0x8B, 0x64, 0x1A, 0xDD, 0x2, 0x31, 0x0, 0xA8, 0x4E, 0xFF, 0x75, 0x3F, 0xD4, 0xDF, 0x10, 0xC7, 0x75, 0xB, 0x8D, 0x68, 0x13, 0x1D, 0xFE, 0x24, 0x12, 0x1C, 0x9C, 0xD6, 0x7F, 0xFA, 0x11, 0xC6, 0x1F, 0x4, 0x78, 0xBE, 0x9E, 0x48, 0x95, 0xBC, 0x16, 0xD5, 0x8B, 0x73, 0x46, 0x50, 0x33, 0x89, 0xA0, 0x6F, 0x20, 0x3, 0x66, 0x91, 0x7B)
                $pest_return = Confirm-YubikeyAttestion -CertificateRequest $pest_input      
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.AttestionValidated | Should -Be $True
        $pest_return.AttestionMatchesCSR | Should -Be $True
    }

}

Describe "Confirm-YubikeyAttestion Attestion/Intermediate Certificates CSPN" -Tag 'Dry' {
    BeforeEach -Scriptblock {
    }

    It -Name "Verify '-AttestionCertificate _PEM_ -IntermediateCertificate _PEM_' works" -Test {
        [string]$pest_attestion_certificate = Get-Content "$PSScriptRoot\TestData\piv_attestion_cspn_attestioncertificate.cer"
        [string]$pest_attestion_intermediate = Get-Content "$PSScriptRoot\TestData\piv_attestion_cspn_intermediatecertificate.cer"
        $pest_return = Confirm-YubikeyAttestion -AttestionCertificate $pest_attestion_certificate -IntermediateCertificate $pest_attestion_intermediate
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.isFIPSSeries | Should -BeFalse
        $pest_return.isCSPNSeries | Should -BeTrue
        $pest_return.AttestionValidated | Should -BeTrue
        $pest_return.AttestionMatchesCSR | Should -BeNullOrEmpty
    }
}

Describe "Confirm-YubikeyAttestion Attestion/Intermediate Certificates FIPS" -Tag 'Dry' {
    BeforeEach -Scriptblock {
    }

    It -Name "Verify '-AttestionCertificate _PEM_ -IntermediateCertificate _PEM_' works" -Test {
        [string]$pest_attestion_certificate = Get-Content "$PSScriptRoot\TestData\piv_attestion_fips_attestioncertificate.cer"
        [string]$pest_attestion_intermediate = Get-Content "$PSScriptRoot\TestData\piv_attestion_fips_intermediatecertificate.cer"
        $pest_return = Confirm-YubikeyAttestion -AttestionCertificate $pest_attestion_certificate -IntermediateCertificate $pest_attestion_intermediate
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.isFIPSSeries | Should -BeTrue
        $pest_return.isCSPNSeries | Should -BeFalse
        $pest_return.AttestionValidated | Should -BeTrue
        $pest_return.AttestionMatchesCSR | Should -BeNullOrEmpty
    }
}

Describe "Confirm-YubikeyAttestion Attestion/Intermediate Certificates" -Tag 'Dry' {
    BeforeEach -Scriptblock {
    }

    It -Name "Verify '-AttestionCertificate _PEM_ -IntermediateCertificate _PEM_' works" -Test {
        [string]$pest_attestion_certificate = Get-Content "$PSScriptRoot\TestData\piv_attestion_attestioncertificate.cer"
        [string]$pest_attestion_intermediate = Get-Content "$PSScriptRoot\TestData\piv_attestion_intermediatecertificate.cer"
        $pest_return = Confirm-YubikeyAttestion -AttestionCertificate $pest_attestion_certificate -IntermediateCertificate $pest_attestion_intermediate
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.isFIPSSeries | Should -BeFalse
        $pest_return.isCSPNSeries | Should -BeFalse
        $pest_return.AttestionValidated | Should -BeTrue
        $pest_return.AttestionMatchesCSR | Should -BeNullOrEmpty
    }

    It -Name "Verify '-AttestionCertificate _X509Certificate2_ -IntermediateCertificate _X509Certificate2_' works" -Test {
        $pest_att = [System.Security.Cryptography.X509Certificates.X509Certificate2]::New("$PSScriptRoot\TestData\piv_attestion_attestioncertificate.cer")
        $pest_int = [System.Security.Cryptography.X509Certificates.X509Certificate2]::New("$PSScriptRoot\TestData\piv_attestion_intermediatecertificate.cer")

        $pest_return = Confirm-YubikeyAttestion -AttestionCertificate $pest_att -IntermediateCertificate $pest_int
        $pest_return | Should -BeOfType powershellYK.Attestion
        $pest_return.Slot | Should -Be 0x9a
        $pest_return.AttestionValidated | Should -BeTrue
        $pest_return.AttestionMatchesCSR | Should -BeNullOrEmpty
    }
}

Describe "Confirm-YubikeyAttestion Errors" -Tag 'Dry' {
       It -Name "Incorrect string on CertificateRequest" -Test {
        {Confirm-YubikeyAttestion -CertificateRequest ""} | Should -Throw
    }
}

AfterAll -Scriptblock {
}
