Describe "OTP HOTP Tests" -Tag "OTP",'OTP-HOTP'  {
    It -Name "Setting should not throw" -Test {
        {$return = Set-YubiKeyOTP -Slot ShortPress -HOTP -SecretKey ([byte[]]@(2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21)) -Confirm:$false} | Should -Not -Throw
    }
    It -Name "Verify that it sets it correctly" -Test {
        $return = Set-YubiKeyOTP -Slot ShortPress -HOTP -SecretKey ([byte[]]@(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20)) -Confirm:$false
	$return.Base32Secret | Should -Be "AEBAGBAFAYDQQCIKBMGA2DQPCAIREEYU"
    }
    It -Name "Verify that it sets it correctly from Base32" -Test {
        $return = Set-YubiKeyOTP -Slot ShortPress -HOTP -Base32Secret 'QRFJ7DTIVASL3PNYXWFIQAQN5RKUJD4U' -Confirm:$false
	$return.SecretKey | Should -Be @(0x84, 0x4a, 0x9f, 0x8e, 0x68, 0xa8, 0x24, 0xbd, 0xbd, 0xb8, 0xbd, 0x8a, 0x88, 0x02, 0x0d, 0xec, 0x55, 0x44, 0x8f, 0x94)
    }
}