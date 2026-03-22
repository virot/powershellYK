Describe "FIDO2 Blob Tests" -Tag @("FIDO2",'FIDO2Blob')  {
    BeforeAll {
        { Connect-YubiKey } | Should -Not -Throw
        { Connect-YubiKeyFIDO2 -PIN (ConvertTo-SecureString -String '123456' -AsPlainText -Force) } | Should -Not -Throw
        { New-YubiKeyFIDO2Credential -RelyingPartyID 'powershellYK-FIDO2-BLOB' -Challenge ([powershellYK.FIDO2.Challenge]::FakeChallange("powershellYK")) -Discoverable:$true -Username 'powershellYKUser' -UserID 0x01 } | Should -Not -Throw
    }
    AfterAll {
        Remove-YubikeyFIDO2Credential -RelayingParty 'powershellYK-FIDO2-BLOB' -Username powershellYKUser
    }
    It -Name "Store file in FIDO2 Blob" -Test {
        { Import-YubiKeyFIDO2Blob -RelyingPartyID 'powershellYK-FIDO2-BLOB' -LargeBlob ".\Pester\TestData\piv_attestion_5_4_3_9a_request.req" } | Should -Not -Throw
    }
}