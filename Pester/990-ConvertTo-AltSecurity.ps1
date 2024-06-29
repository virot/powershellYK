BeforeAll -Scriptblock {
}

Describe "ConvertTo-AltSecurity" -Tag @("Dry",'CertificateTransform','CertificateValidate') {
    BeforeEach -Scriptblock {
        $pest_return = $Null
    }

    It -Name "Verify _fullfile_'" -Test {
        {ConvertTo-AltSecurity -Certificate "$PSScriptRoot\TestData\piv_certificate_selfsigned.cer"} | Should -Not -Throw
        $pest_return = ConvertTo-AltSecurity -Certificate "$PSScriptRoot\TestData\piv_certificate_selfsigned.cer"
        $pest_return | Should -BeOfType powershellYK.AlternativeIdentites
        $pest_return.sshAuthorizedkey | Should -Be "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCajO4LNYvGOmTb++u/8uxzOfpoTr8+RZoKXYd1hV0uQFJqAfD1MY4KYpfZcMhE+QiAwM0jtFFRPbtBfUYMI3fVc0aTRXNiWtBWXlZvBIAVz3GwAqxzthDgOZvVZ80itEJgs6FnhjVDj+q5Ceq2nV89dHO1EWEDZWzq9kxL0DgRkwccTTtkPNqIt2fkqhGzsx2IRGBvzEQdwL+9fFOcPP9kQ8dcXIXkjgNHACp21Te2vxVm9SDHoZrz+8zkc9P6SE2lAXLQMKp+AXT1Zpj8D3NJMqxQrv/NgjJ83+HFb0KVZ+X6adAqhVKtjNcQDgWAebL8UQS4arekJI/MNSBgHi+3 CN=SubjectName to be supplied by Server, O=Fake"
    }

    It -Name "Verify _certificate_'" -Test {
        $pest_cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new("$PSScriptRoot\TestData\piv_certificate_selfsigned.cer")
        $pest_return = ConvertTo-AltSecurity -Certificate $pest_cert
        $pest_return | Should -BeOfType powershellYK.AlternativeIdentites
        $pest_return.sshAuthorizedkey | Should -Be "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCajO4LNYvGOmTb++u/8uxzOfpoTr8+RZoKXYd1hV0uQFJqAfD1MY4KYpfZcMhE+QiAwM0jtFFRPbtBfUYMI3fVc0aTRXNiWtBWXlZvBIAVz3GwAqxzthDgOZvVZ80itEJgs6FnhjVDj+q5Ceq2nV89dHO1EWEDZWzq9kxL0DgRkwccTTtkPNqIt2fkqhGzsx2IRGBvzEQdwL+9fFOcPP9kQ8dcXIXkjgNHACp21Te2vxVm9SDHoZrz+8zkc9P6SE2lAXLQMKp+AXT1Zpj8D3NJMqxQrv/NgjJ83+HFb0KVZ+X6adAqhVKtjNcQDgWAebL8UQS4arekJI/MNSBgHi+3 CN=SubjectName to be supplied by Server, O=Fake"
    }

    It -Name "Verify [psobject] _certificate_'" -Test {
        $pest_cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new("$PSScriptRoot\TestData\piv_certificate_selfsigned.cer")
        $pest_return = ConvertTo-AltSecurity -Certificate [psobject]$pest_cert
        $pest_return | Should -BeOfType powershellYK.AlternativeIdentites
        $pest_return.sshAuthorizedkey | Should -Be "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCajO4LNYvGOmTb++u/8uxzOfpoTr8+RZoKXYd1hV0uQFJqAfD1MY4KYpfZcMhE+QiAwM0jtFFRPbtBfUYMI3fVc0aTRXNiWtBWXlZvBIAVz3GwAqxzthDgOZvVZ80itEJgs6FnhjVDj+q5Ceq2nV89dHO1EWEDZWzq9kxL0DgRkwccTTtkPNqIt2fkqhGzsx2IRGBvzEQdwL+9fFOcPP9kQ8dcXIXkjgNHACp21Te2vxVm9SDHoZrz+8zkc9P6SE2lAXLQMKp+AXT1Zpj8D3NJMqxQrv/NgjJ83+HFb0KVZ+X6adAqhVKtjNcQDgWAebL8UQS4arekJI/MNSBgHi+3 CN=SubjectName to be supplied by Server, O=Fake"
    }

    It -Name "Verify _pemdata_'" -Test {
        $pest_cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new("$PSScriptRoot\TestData\piv_certificate_selfsigned.cer")
        $pest_return = ConvertTo-AltSecurity -Certificate $pest_cert.ExportCertificatePem()
        $pest_return | Should -BeOfType powershellYK.AlternativeIdentites
        $pest_return.sshAuthorizedkey | Should -Be "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABAQCajO4LNYvGOmTb++u/8uxzOfpoTr8+RZoKXYd1hV0uQFJqAfD1MY4KYpfZcMhE+QiAwM0jtFFRPbtBfUYMI3fVc0aTRXNiWtBWXlZvBIAVz3GwAqxzthDgOZvVZ80itEJgs6FnhjVDj+q5Ceq2nV89dHO1EWEDZWzq9kxL0DgRkwccTTtkPNqIt2fkqhGzsx2IRGBvzEQdwL+9fFOcPP9kQ8dcXIXkjgNHACp21Te2vxVm9SDHoZrz+8zkc9P6SE2lAXLQMKp+AXT1Zpj8D3NJMqxQrv/NgjJ83+HFb0KVZ+X6adAqhVKtjNcQDgWAebL8UQS4arekJI/MNSBgHi+3 CN=SubjectName to be supplied by Server, O=Fake"
    }

}

AfterAll -Scriptblock {
}