using System.Diagnostics.Eventing.Reader;
using System.Management.Automation;           // Windows PowerShell namespace.
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using VirotYubikey.support;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Sample.PivSampleCode;
using static System.Security.Cryptography.X509Certificates.CertificateRequest;

namespace VirotYubikey.Cmdlets.Other
{
    [Cmdlet(VerbsData.ConvertTo, "AltSecurity")]
    public class ConvertToConvertToAltSecurityCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0 , HelpMessage = "Certificate to extract info from")]
        public PSObject? Certificate { get; set; }
        private X509Certificate2? _certificate = null;
        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            WriteDebug($"Trying to load certificate from {Certificate.BaseObject.GetType().FullName}");
            if (Certificate.BaseObject is X509Certificate2)
            {
                _certificate = Certificate.BaseObject as X509Certificate2;
            }
            else if (Certificate.BaseObject is X509Certificate)
            {
                _certificate = new X509Certificate2((X509Certificate)Certificate.BaseObject);
            }
            else if (Certificate.BaseObject is string)
            {
                _certificate = new X509Certificate2(Certificate.BaseObject! as string);
            }
            else
            {
                throw new Exception($"Unknown certificate format, {Certificate.BaseObject.GetType().FullName}");
            }

            if (_certificate is not null)
            {
                WriteDebug("Certificate successfully loaded");
                if (_certificate.PublicKey.Oid.FriendlyName == "RSA")
                {
                    WriteDebug("Certificate public key is of type RSA");
                    WriteObject(GenerateIdentifier.SSHIdentifier(_certificate.PublicKey.GetRSAPublicKey(), _certificate.Subject));
                }
                else if (_certificate.PublicKey.Oid.FriendlyName == "ECC")
                {
                    WriteDebug("Certificate public key is of type ECC");
                    ECParameters publicKeyParam = _certificate.PublicKey.GetECDsaPublicKey().ExportParameters(false);
                    WriteDebug($"ECC Curve: {publicKeyParam.Curve.Oid.FriendlyName}");

                    WriteObject(GenerateIdentifier.SSHIdentifier(_certificate.PublicKey.GetECDsaPublicKey(), _certificate.Subject));
                }
                else
                {
                    throw new Exception("Unknown publickey format");
                }
            }
            else
            {
                throw new Exception("Certificate not loaded");
            }


        }

    }
}