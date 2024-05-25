﻿using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using VirotYubikey.support;
using Yubico.YubiKey.Sample.PivSampleCode;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVSelfSign", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVSelfSignCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Sign a self signed cert for slot")]

        public byte Slot { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subjectname of certificate")]

        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        protected override void ProcessRecord()
        {
            CertificateRequest request;
            X509SignatureGenerator signer;
            X500DistinguishedName dn;

            if (YubiKeyModule._pivSession is null)
            {
                //throw new Exception("PIV not connected, use Connect-YubikeyPIV first");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyPIV");
                    myPowersShellInstance.Invoke();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            PivPublicKey? publicKey = null;
            try
            {
                publicKey = YubiKeyModule._pivSession!.GetMetadata(Slot).PublicKey;
                if (publicKey is null)
                {
                    throw new Exception("Public key is null");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get public key for slot 0x{Slot.ToString("X2")}, does there exist a key?", e);
            }


            using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(publicKey);

            if (publicKey is PivRsaPublicKey)
            {
                request = new CertificateRequest(Subjectname, (RSA)dotNetPublicKey, HashAlgorithm, RSASignaturePadding.Pkcs1);
            }
            else
            {
                HashAlgorithm = publicKey.Algorithm switch
                {
                    PivAlgorithm.EccP256 => HashAlgorithmName.SHA256,
                    PivAlgorithm.EccP384 => HashAlgorithmName.SHA384,
                    _ => throw new Exception("Unknown PublicKey algorithm")
                };
                WriteDebug($"Using Hash based on ECC size: {HashAlgorithm.ToString()}");
                request = new CertificateRequest(Subjectname, (ECDsa)dotNetPublicKey, HashAlgorithm);
            }

            X509BasicConstraintsExtension x509BasicConstraintsExtension = new X509BasicConstraintsExtension(true, true, 2, true);
            X509KeyUsageExtension x509KeyUsageExtension = new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, false);
            request.CertificateExtensions.Add(x509BasicConstraintsExtension);
            request.CertificateExtensions.Add(x509KeyUsageExtension);

            DateTimeOffset notBefore = DateTimeOffset.Now;
            DateTimeOffset notAfter = notBefore.AddYears(10);
            byte[] serialNumber = new byte[] { 0x01 };

            if (publicKey is PivRsaPublicKey)
            {
                signer = new YubiKeySignatureGenerator(YubiKeyModule._pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
            }
            else
            {
                signer = new YubiKeySignatureGenerator(YubiKeyModule._pivSession, Slot, publicKey);
            }

            try
            {
                dn = new X500DistinguishedName(Subjectname);
            } catch (Exception e) { throw new Exception("Failed to create X500DistinguishedName", e);}
            X509Certificate2 selfCert = request.Create(dn, signer, notBefore, notAfter, serialNumber);

            bool certExists = false;
            try
            {
                X509Certificate2 certtest = YubiKeyModule._pivSession.GetCertificate(Slot);
                certExists = true;
            }
            catch { }

            if (!certExists || ShouldProcess($"Certificate in slot 0x{Slot.ToString("X2")}", "New"))
            {
                PowerShell myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Import-YubikeyPIVCertificate");
                myPowersShellInstance.AddParameter("Slot", Slot);
                myPowersShellInstance.AddParameter("Certificate", selfCert);
                myPowersShellInstance.Invoke();

            }
            WriteDebug("ProcessRecord in New-YubikeyPIVSelfSign");

        }


        protected override void EndProcessing()
        {
        }
    }
}