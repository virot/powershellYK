/// <summary>
/// Offline-verifies a previewSign signature produced by New-YubiKeyFIDO2Signature.
/// Checks ECDSA-P256 over ToBeSigned using DerivedPublicKey, optionally re-hashes
/// the original data and compares it to ToBeSigned, and decodes the generated-key
/// attestation object. UserPresence/UserVerification report the signing key's fixed
/// UP/UV policy (from the previewSign extension flags), not the creation ceremony.
/// Does not require a YubiKey to be present.
///
/// NOTE: This cmdlet uses the Yubico previewSign extension. Its algorithm and
/// algorithm ID (-65539) are not final and may change before general
/// availability. Do not use this in production.
///
/// .EXAMPLE
/// $key = New-YubiKeyFIDO2Signature -InputData "data"
/// $key | Confirm-YubiKeyFIDO2Signature -InputData "data"
/// Verifies the signature and that the data hashes to ToBeSigned.
///
/// .EXAMPLE
/// Confirm-YubiKeyFIDO2Signature -JsonPath .\previewsign.json -Path .\document.pdf
/// Loads a stored PreviewSignKey JSON and checks it against the original file.
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using powershellYK.support.transform;
using powershellYK.support.validators;
using powershellYK.FIDO2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsLifecycle.Confirm, "YubiKeyFIDO2Signature", DefaultParameterSetName = "PreviewSignKey")]
    [OutputType(typeof(PreviewSignVerificationResult))]
    public class ConfirmYubiKeyFIDO2SignatureCommand : PSCmdlet
    {
        // Shown on every invocation: previewSign is not a stable, GA algorithm
        private const string PreviewSignMaturityWarning =
            "This cmdlet uses the Yubico previewSign extension. Its algorithm and algorithm ID (-65539) are not final and may change before general availability. Do not use this in production.";

        // Pipeline PreviewSignKey from New-YubiKeyFIDO2Signature
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "PreviewSignKey to verify.", ParameterSetName = "PreviewSignKey")]
        public PreviewSignKey? PreviewSignKey { get; set; }

        // JSON produced by ToJson() / the host store
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Path to PreviewSignKey JSON (ToJson / ~/.powershellYK store).", ParameterSetName = "Json")]
        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        public FileInfo? JsonPath { get; set; }

        // Optional original data to re-hash and compare to ToBeSigned
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Original data (hex string or byte[]) to re-hash and compare to ToBeSigned.")]
        [TransformHexInput]
        public byte[]? InputData { get; set; }

        // Optional original file to re-hash and compare to ToBeSigned
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Original file to re-hash and compare to ToBeSigned.")]
        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        public FileInfo? Path { get; set; }

        // Initialize processing
        protected override void BeginProcessing()
        {
            WriteWarning(PreviewSignMaturityWarning);
        }

        // Verify the supplied PreviewSignKey or JSON file
        protected override void ProcessRecord()
        {
            if (InputData is not null && Path is not null)
            {
                throw new ArgumentException("Specify either -InputData or -Path, not both.");
            }

            PreviewSignKey key;
            if (ParameterSetName == "Json")
            {
                string resolved = GetUnresolvedProviderPathFromPSPath(JsonPath!.FullName);
                string json;
                try
                {
                    json = File.ReadAllText(resolved);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Failed to read previewSign key JSON from '{resolved}'.", ex);
                }
                key = PreviewSignKey.FromJson(json);
            }
            else
            {
                key = PreviewSignKey!;
            }

            if (key.SignatureBytes is null || key.SignatureBytes.Length == 0)
            {
                throw new ArgumentException("The PreviewSignKey does not contain a Signature to verify.");
            }
            if (key.ToBeSignedBytes is null || key.ToBeSignedBytes.Length == 0)
            {
                throw new ArgumentException("The PreviewSignKey does not contain ToBeSigned digest bytes.");
            }
            if (key.DerivedPublicKeySec1 is null || key.DerivedPublicKeySec1.Length == 0)
            {
                throw new ArgumentException("The PreviewSignKey does not contain DerivedPublicKey. This store was created before the verify key was persisted; sign once more with New-YubiKeyFIDO2Signature so DerivedPublicKey is recorded. The original IKM cannot be recovered.");
            }

            byte[]? documentBytes = null;
            if (Path is not null)
            {
                string resolvedDoc = GetUnresolvedProviderPathFromPSPath(Path.FullName);
                try
                {
                    documentBytes = File.ReadAllBytes(resolvedDoc);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Failed to read input file from '{Path}'.", ex);
                }
            }
            else if (InputData is not null)
            {
                documentBytes = InputData;
            }

            bool signatureValid = VerifyEcdsaP256(key.DerivedPublicKeySec1, key.ToBeSignedBytes, key.SignatureBytes);

            bool? digestMatches = null;
            if (documentBytes is not null)
            {
                byte[] computed = ComputeDigest(documentBytes, key.HashAlgorithm);
                digestMatches = CryptographicOperations.FixedTimeEquals(computed, key.ToBeSignedBytes);
            }

            string? aaguid = null;
            bool? userPresence = null;
            bool? userVerification = null;

            if (key.AttestationObjectBytes is not null && key.AttestationObjectBytes.Length > 0)
            {
                try
                {
                    var att = new AttestationObject(key.AttestationObjectBytes);
                    var authData = att.AuthenticatorData;
                    aaguid = FormatAaguid(authData);

                    // The signing key's fixed UP/UV policy lives in the "previewSign"
                    // extension output of the generated-key attestation, not the authData
                    // UP/UV bits (which describe the creation ceremony). Leave null when the
                    // flags output is absent.
                    if (powershellYK.support.FIDO2.PreviewSign.TryGetSigningKeyPolicy(authData, out bool requiresUserPresence, out bool requiresUserVerification))
                    {
                        userPresence = requiresUserPresence;
                        userVerification = requiresUserVerification;
                    }
                }
                catch (Exception)
                {
                    aaguid = null;
                    userPresence = null;
                    userVerification = null;
                }
            }

            WriteObject(new PreviewSignVerificationResult(
                signatureValid: signatureValid,
                digestMatches: digestMatches,
                aaguid: aaguid,
                userPresence: userPresence,
                userVerification: userVerification));
        }

        // Verify DER (or raw P1363) ECDSA-P256 over a digest with an uncompressed SEC1 public key
        private static bool VerifyEcdsaP256(byte[] sec1, byte[] digest, byte[] signature)
        {
            if (sec1.Length != 65 || sec1[0] != 0x04)
            {
                return false;
            }

            try
            {
                using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                ecdsa.ImportParameters(new ECParameters
                {
                    Curve = ECCurve.NamedCurves.nistP256,
                    Q = new ECPoint
                    {
                        X = sec1.AsSpan(1, 32).ToArray(),
                        Y = sec1.AsSpan(33, 32).ToArray(),
                    },
                });

                if (ecdsa.VerifyHash(digest, signature, DSASignatureFormat.Rfc3279DerSequence))
                {
                    return true;
                }
                if (signature.Length == 64)
                {
                    return ecdsa.VerifyHash(digest, signature, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
                }
                return false;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        // Format AAGUID as a GUID string when 16 bytes are present
        private static string? FormatAaguid(AuthenticatorData authData)
        {
            ReadOnlyMemory<byte>? maybe = authData.Aaguid;
            if (maybe is null)
            {
                return null;
            }
            byte[] bytes = maybe.Value.ToArray();
            if (bytes.Length == 0)
            {
                return null;
            }
            if (bytes.Length == 16)
            {
                return new Guid(bytes).ToString();
            }
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // Hash input with the algorithm recorded on the PreviewSignKey
        private static byte[] ComputeDigest(byte[] data, string digest)
        {
            return digest.ToUpperInvariant() switch
            {
                "SHA256" => SHA256.HashData(data),
                "SHA384" => SHA384.HashData(data),
                "SHA512" => SHA512.HashData(data),
                _ => throw new ArgumentException($"Unsupported digest algorithm '{digest}'."),
            };
        }
    }
}
