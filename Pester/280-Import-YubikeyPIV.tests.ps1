BeforeAll -Scriptblock {
    Reset-YubikeyPIV -Confirm:$false
    Connect-YubikeyPIV -PIN (ConvertTo-SecureString -String "123456" -AsPlainText -Force)
}

#openssl req -x509 -newkey rsa:2048 -keyout rsa_2048_key_withoutpassword.pem -out rsa_2048_cert.pem -sha256 -days 3650 -nodes -subj "/C=SE/O=powershellYK/OU=Pester tests/CN=2048"
#openssl req -x509 -newkey rsa:4096 -keyout rsa_4096_key_withoutpassword.pem -out rsa_4096_cert.pem -sha256 -days 3650 -nodes -subj "/C=SE/O=powershellYK/OU=Pester tests/CN=4096"
#openssl req -x509 -newkey ec -pkeyopt ec_paramgen_curve:secp384r1 -keyout ecc_secp384r1_key_withoutpassword.pem -out ecc_secp384r1_cert.pem -sha256 -days 3650 -nodes -subj "/C=SE/O=powershellYK/OU=Pester tests/CN=secp384r1"

#openssl.exe req -x509 -key rsa_2048_key_withoutpassword.pem -out rsa_2048_cert2.pem -sha256 -days 3650 -nodes -subj "/C=SE/O=powershellYK/OU=Pester tests/CN=2048_2"
#openssl.exe req -x509 -key ecc_secp384r1_key_withoutpassword.pem -out ecc_secp384r1_cert_2.pem -sha256 -days 3650 -nodes -subj "/C=SE/O=powershellYK/OU=Pester tests/CN=secp384r1_2"

#openssl rsa -in rsa_2048_key_withoutpassword.pem -des3 -out rsa_2048_key_des3_password=password.pem -passout pass:password
#openssl rsa -in rsa_2048_key_withoutpassword.pem -aes256 -out rsa_2048_key_aes256_password=password.pem -passout pass:password

#openssl ec -in ecc_secp384r1_key_withoutpassword.pem -des3 -out ecc_secp384r1_key_des3_password=password.pem -passout pass:password
#openssl ec -in ecc_secp384r1_key_withoutpassword.pem -aes256 -out ecc_secp384r1_key_aes256_password=password.pem -passout pass:password

#openssl pkcs12 -export -out rsa_2048_password=password.p12 -inkey rsa_2048_key_withoutpassword.pem -in rsa_2048_cert.pem -passout pass:password
#openssl pkcs12 -export -out ecc_secp384r1_password=password.p12 -inkey ecc_secp384r1_key_withoutpassword.pem -in ecc_secp384r1_cert.pem -passout pass:password


Describe "Import-YubikeyPIV PrivateKey" -Tag "Import-YubikeyPIV",'PIV' {
    It -Name "RSA2048 '-PrivateKeyPath _fullpath_'" -Test {
        {Get-YubikeyPIV -Slot 0x9a} | Should -Throw
	{Import-YubikeyPIV -Slot 0x9a -PrivateKeyPath "$PSScriptRoot\TestData\rsa_2048_key_withoutpassword.pem"} | Should -Not -Throw
        {Get-YubikeyPIV -Slot 0x9a} | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9a).PublicKey.GetType().Fullname | Should -Be 'Yubico.YubiKey.Cryptography.RSAPublicKey'
    }

    It -Name "ECC P384 '-PrivateKeyPath _fullpath_'" -Test {
	{Import-YubikeyPIV -Slot 0x9a -PrivateKeyPath "$PSScriptRoot\TestData\ecc_secp384r1_key_withoutpassword.pem" -Confirm:$False} | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9a).PublicKey.GetType().Fullname | Should -Be 'Yubico.YubiKey.Cryptography.ECPublicKey'
    }

    It -Name "RSA2048 DES3 '-PrivateKeyPath _fullpath_ -Password _password_'" -Test {
	{Import-YubikeyPIV -Slot 0x9a -PrivateKeyPath "$PSScriptRoot\TestData\rsa_2048_key_des3_password=password.pem" -Password (ConvertTo-SecureString -String "password" -AsPlainText -Force) -Confirm:$False} | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9a).PublicKey.GetType().Fullname | Should -Be 'Yubico.YubiKey.Cryptography.RSAPublicKey'
    }

    #It -Name "ECC P384 '-PrivateKeyPath _fullpath_ -Password _password_'" -Test {
    #    {Import-YubikeyPIV -Slot 0x9a -PrivateKeyPath "$PSScriptRoot\TestData\ecc_secp384r1_key_des3_password=password.pem" -Password (ConvertTo-SecureString -String "password" -AsPlainText -Force) -Confirm:$False} | Should -Not -Throw
    #    (Get-YubikeyPIV -Slot 0x9a).PublicKey.GetType().Fullname | Should -Be 'Yubico.YubiKey.Cryptography.ECPublicKey'
    #}

    It -Name "RSA2048 AES256 '-PrivateKeyPath _fullpath_ -Password _password_'" -Test {
	New-YubikeyPIVKey -Slot 0x9a -Algorithm EcP384 -Confirm:$false
	{Import-YubikeyPIV -Slot 0x9a -PrivateKeyPath "$PSScriptRoot\TestData\rsa_2048_key_aes256_password=password.pem" -Password (ConvertTo-SecureString -String "password" -AsPlainText -Force) -Confirm:$False} | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9a).PublicKey.GetType().Fullname | Should -Be 'Yubico.YubiKey.Cryptography.RSAPublicKey'
    }

}

Describe "Import-YubikeyPIV P12" -Tag "Import-YubikeyPIV" {
    It -Name "RSA2048 '-P12Path _fullpath_ -Password _password_'" -Test {
        {Get-YubikeyPIV -Slot 0x9c} | Should -Throw
	{Import-YubikeyPIV -Slot 0x9c -P12Path "$PSScriptRoot\TestData\rsa_2048_password=password.p12" -Confirm:$False } | Should -Throw
	{Import-YubikeyPIV -Slot 0x9c -P12Path "$PSScriptRoot\TestData\rsa_2048_password=password.p12" -Password (ConvertTo-SecureString -String "password" -AsPlainText -Force) -Confirm:$False } | Should -Not -Throw
        {Get-YubikeyPIV -Slot 0x9c} | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9c).PublicKey.GetType().Fullname | Should -Be 'Yubico.YubiKey.Cryptography.RSAPublicKey'
    }
    # Dont check whill ECDsaCng breaks plaintext exports
    #It -Name "ECC P384 '-P12Path _fullpath_ -Password _password_'" -Test {
    #    {Get-YubikeyPIV -Slot 0x9d} | Should -Throw
    #    {Import-YubikeyPIV -Slot 0x9d -P12Path "$PSScriptRoot\TestData\ecc_secp384r1_password=password.p12" -Confirm:$False } | Should -Throw
    #    {Import-YubikeyPIV -Slot 0x9d -P12Path "$PSScriptRoot\TestData\ecc_secp384r1_password=password.p12" -Password (ConvertTo-SecureString -String "password" -AsPlainText -Force) -Confirm:$False } | Should -Not -Throw
    #    {Get-YubikeyPIV -Slot 0x9d} | Should -Not -Throw
    #    (Get-YubikeyPIV -Slot 0x9d).PublicKey.GetType().Fullname | Should -Be 'Yubico.YubiKey.Cryptography.ECPublicKey'
    #}
}

Describe "Import-YubikeyPIV Certificate" -Tag "Import-YubikeyPIV" {
    It -Name "RSA2048 -Certificate _fullname_" -Test {
	{Import-YubikeyPIV -Slot 0x9d -Certificate "$PSScriptRoot\TestData\rsa_2048_cert.pem" -Confirm:$False } | Should -Throw
	{Import-YubikeyPIV -Slot 0x9c -Certificate "$PSScriptRoot\TestData\rsa_2048_cert2.pem" -Confirm:$False } | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9c).Certificate.Thumbprint | Should -Be '790a6e582849266caa21ec8ad446f7abe08fe47c'
	{Import-YubikeyPIV -Slot 0x9c -Certificate "$PSScriptRoot\TestData\rsa_2048_cert.pem" -Confirm:$False } | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9c).Certificate.Thumbprint | Should -Be '68c6a65f3e5fddbeb5ed491b113feb1a450be675'
    }
    It -Name "ECC P384 -Certificate _fullname_" -Test {
        {Import-YubikeyPIV -Slot 0x9d -PrivateKeyPath "$PSScriptRoot\TestData\ecc_secp384r1_key_withoutpassword.pem" -Confirm:$False} | Should -Not -Throw
	{Import-YubikeyPIV -Slot 0x9c -Certificate "$PSScriptRoot\TestData\ecc_secp384r1_cert.pem" -Confirm:$False } | Should -Throw
	{Import-YubikeyPIV -Slot 0x9d -Certificate "$PSScriptRoot\TestData\ecc_secp384r1_cert.pem" -Confirm:$False } | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9d).Certificate.Thumbprint | Should -Be '1162F3FB723E574A8033EF9905C6D6A160B48063'
	{Import-YubikeyPIV -Slot 0x9d -Certificate "$PSScriptRoot\TestData\ecc_secp384r1_cert_2.pem" -Confirm:$False } | Should -Not -Throw
	(Get-YubikeyPIV -Slot 0x9d).Certificate.Thumbprint | Should -Be '585272ea6fc1a8d7a402e97a2e0153c6c8284d23'
    }
}

Describe "Import-YubikeyPIV PrivateKey&Certificate" -Tag "Import-YubikeyPIV" {
    It -Name "-CertificateRequest _shortfile_" -Test {
    }
}

AfterAll -Scriptblock {
}