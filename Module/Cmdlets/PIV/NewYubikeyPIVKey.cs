﻿using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;
using System.Collections.ObjectModel;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVKeyCommand : PSCmdlet, IDynamicParameters
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [TransformPivSlot()]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to create a new key for")]
        public byte Slot { get; set; }

        //[Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Algoritm")]
        //public String Algorithm { get; set; }

        //[ValidateSet("Default", "Never", "None", "Once", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "PinPolicy")]
        public PivPinPolicy PinPolicy { get; set; } = PivPinPolicy.Default;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "TouchPolicy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        [Parameter(Mandatory = false, HelpMessage = "Returns an object that represents the item with which you're working. By default, this cmdlet doesn't generate any output.")]
        public SwitchParameter PassThru { get; set; }

        public object GetDynamicParameters()
        {
            //Add the Algorithm parameter
            var availableAlgorithms = new List<String>();
            YubiKeyModule.ConnectYubikey();
            if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa1024)) { availableAlgorithms.Add("Rsa1024"); };
            if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa2048)) { availableAlgorithms.Add("Rsa2048"); };
            if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa3072)) { availableAlgorithms.Add("Rsa3072"); };
            if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa4096)) { availableAlgorithms.Add("Rsa4096"); };
            if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP256)) { availableAlgorithms.Add("EccP256"); };
            if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP384)) { availableAlgorithms.Add("EccP384"); };
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();

            var algorithmCollection = new Collection<Attribute>() {
                new ParameterAttribute() { Mandatory = true, HelpMessage = "What algorithm to use", ParameterSetName = "__AllParameterSets", ValueFromPipeline = false },
                new ValidateSetAttribute((availableAlgorithms).ToArray())};
            var runtimeDefinedAlgorithms = new RuntimeDefinedParameter("Algorithm", typeof(PivAlgorithm), algorithmCollection);
            runtimeDefinedParameterDictionary.Add("Algorithm", runtimeDefinedAlgorithms);
            return runtimeDefinedParameterDictionary;
        }
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                bool keyExists = false;
                try
                {
                    PivPublicKey pubkey = pivSession.GetMetadata(Slot).PublicKey;
                    keyExists = true;
                }
                catch { }

                if (!keyExists || ShouldProcess($"Slot 0x{Slot.ToString("X2")}", "New"))
                {
                    WriteDebug("ProcessRecord in New-YubikeyPIVKey");
                    PivPublicKey publicKey = pivSession.GenerateKeyPair(Slot, (PivAlgorithm)this.MyInvocation.BoundParameters["Algorithm"], PinPolicy, TouchPolicy);
                    if (publicKey is not null)
                    {
                        if (PassThru.IsPresent)
                        {
                            using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(publicKey);
                            if (publicKey is PivRsaPublicKey)
                            {
                                WriteObject((RSA)dotNetPublicKey);
                            }
                            else
                            {
                                WriteObject((ECDsa)dotNetPublicKey);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Could not create keypair");
                    }
                }
            }
        }
    }
}