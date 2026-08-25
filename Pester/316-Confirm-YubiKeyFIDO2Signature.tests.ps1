Describe "Confirm-YubiKeyFIDO2Signature" -Tag "Without-YubiKey","Dry" {

    BeforeAll {
        function Copy-P256Coordinate {
            param([byte[]]$Coord, [byte[]]$Dest, [int]$Offset)
            if ($Coord.Length -ge 32) {
                [System.Buffer]::BlockCopy($Coord, $Coord.Length - 32, $Dest, $Offset, 32)
            } else {
                [System.Buffer]::BlockCopy($Coord, 0, $Dest, $Offset + (32 - $Coord.Length), $Coord.Length)
            }
        }

        function New-EcdsaPreviewMaterial {
            param([byte[]]$Data)
            $ecdsa = [System.Security.Cryptography.ECDsa]::Create([System.Security.Cryptography.ECCurve+NamedCurves]::nistP256)
            $digest = [System.Security.Cryptography.SHA256]::HashData($Data)
            $signature = $ecdsa.SignHash($digest, [System.Security.Cryptography.DSASignatureFormat]::Rfc3279DerSequence)
            $p = $ecdsa.ExportParameters($false)
            $sec1 = New-Object byte[] 65
            $sec1[0] = 0x04
            Copy-P256Coordinate -Coord $p.Q.X -Dest $sec1 -Offset 1
            Copy-P256Coordinate -Coord $p.Q.Y -Dest $sec1 -Offset 33
            return @{ Digest = $digest; Signature = $signature; Sec1 = $sec1 }
        }

        function New-PreviewSignTestKey {
            param(
                [byte[]]$DerivedPublicKey,
                [byte[]]$Signature,
                [byte[]]$ToBeSigned,
                [byte[]]$AttestationObject,
                [string]$RelyingPartyID = "previewsign.powershellyk"
            )
            $cred = [powershellYK.FIDO2.CredentialID]::new("aabbccdd")
            $handle = [byte[]](1..16)
            $seed = [byte[]](2..17)
            return [powershellYK.FIDO2.PreviewSignKey]::new(
                $cred,
                $RelyingPartyID,
                [Yubico.YubiKey.Fido2.Cose.CoseAlgorithmIdentifier](-65539),
                "SHA256",
                $ToBeSigned,
                $handle,
                $seed,
                $AttestationObject,
                $Signature,
                $DerivedPublicKey)
        }

        function New-NoneAttestationObject {
            param([string]$RelyingPartyID)
            $rpHash = [System.Security.Cryptography.SHA256]::HashData([System.Text.Encoding]::UTF8.GetBytes($RelyingPartyID))
            $authData = New-Object byte[] 37
            [System.Buffer]::BlockCopy($rpHash, 0, $authData, 0, 32)
            $authData[32] = 0x01
            $cbor = New-Object System.Collections.Generic.List[byte]
            $cbor.Add(0xa3)
            $cbor.Add(0x01)
            $cbor.AddRange([byte[]](0x64, 0x6e, 0x6f, 0x6e, 0x65))
            $cbor.Add(0x02)
            $cbor.Add(0x58)
            $cbor.Add(0x25)
            $cbor.AddRange($authData)
            $cbor.Add(0x03)
            $cbor.Add(0xa0)
            return [byte[]]$cbor.ToArray()
        }
    }

    It "Verifies a matching ECDSA signature and digest" {
        $data = [System.Text.Encoding]::UTF8.GetBytes("hello")
        $mat = New-EcdsaPreviewMaterial -Data $data
        $att = New-NoneAttestationObject -RelyingPartyID "previewsign.powershellyk"
        $key = New-PreviewSignTestKey -DerivedPublicKey $mat.Sec1 -Signature $mat.Signature -ToBeSigned $mat.Digest -AttestationObject $att

        $result = $key | Confirm-YubiKeyFIDO2Signature -InputData $data -WarningAction SilentlyContinue
        $result.Valid | Should -Be $true
        $result.SignatureValid | Should -Be $true
        $result.DigestMatches | Should -Be $true
        $result.AttestationDecoded | Should -Be $true
        $result.AttestationFormat | Should -Be "none"
        $result.RpIdHashMatches | Should -Be $true
        $result.UserPresence | Should -Be $true
    }

    It "Reports DigestMatches false when the input does not match ToBeSigned" {
        $data = [System.Text.Encoding]::UTF8.GetBytes("hello")
        $mat = New-EcdsaPreviewMaterial -Data $data
        $key = New-PreviewSignTestKey -DerivedPublicKey $mat.Sec1 -Signature $mat.Signature -ToBeSigned $mat.Digest -AttestationObject $null

        $tampered = [System.Text.Encoding]::UTF8.GetBytes("tampered")
        $result = $key | Confirm-YubiKeyFIDO2Signature -InputData $tampered -WarningAction SilentlyContinue
        $result.SignatureValid | Should -Be $true
        $result.DigestMatches | Should -Be $false
        $result.Valid | Should -Be $false
    }

    It "Decodes fmt=none attestation without a document" {
        $data = [System.Text.Encoding]::UTF8.GetBytes("hello")
        $mat = New-EcdsaPreviewMaterial -Data $data
        $att = New-NoneAttestationObject -RelyingPartyID "previewsign.powershellyk"
        $key = New-PreviewSignTestKey -DerivedPublicKey $mat.Sec1 -Signature $mat.Signature -ToBeSigned $mat.Digest -AttestationObject $att

        $result = $key | Confirm-YubiKeyFIDO2Signature -WarningAction SilentlyContinue
        $result.DigestMatches | Should -Be $null
        $result.AttestationDecoded | Should -Be $true
        $result.AttestationFormat | Should -Be "none"
        ($result.Notes -join " ") | Should -Match "none"
    }

    It "Throws when DerivedPublicKey is missing" {
        $digest = [byte[]](1..32)
        $signature = [byte[]](1..70)
        $key = New-PreviewSignTestKey -DerivedPublicKey $null -Signature $signature -ToBeSigned $digest -AttestationObject $null
        { $key | Confirm-YubiKeyFIDO2Signature -WarningAction SilentlyContinue } | Should -Throw -ExpectedMessage '*DerivedPublicKey*'
    }

    It "Loads JSON via -JsonPath and checks a file with -Path" {
        $data = [System.Text.Encoding]::UTF8.GetBytes("hello")
        $mat = New-EcdsaPreviewMaterial -Data $data
        $key = New-PreviewSignTestKey -DerivedPublicKey $mat.Sec1 -Signature $mat.Signature -ToBeSigned $mat.Digest -AttestationObject $null
        $jsonPath = Join-Path $TestDrive "previewsign-test.json"
        $docPath = Join-Path $TestDrive "document.bin"
        Set-Content -Path $jsonPath -Value $key.ToJson() -Encoding utf8
        [System.IO.File]::WriteAllBytes($docPath, $data)

        $result = Confirm-YubiKeyFIDO2Signature -JsonPath $jsonPath -Path $docPath -WarningAction SilentlyContinue
        $result.Valid | Should -Be $true
        $result.SignatureValid | Should -Be $true
        $result.DigestMatches | Should -Be $true
    }
}
