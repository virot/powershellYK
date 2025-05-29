/// <summary>
/// Represents a PIV slot with validation and conversion capabilities.
/// Handles slot number validation and conversion between different formats.
/// 
/// .EXAMPLE
/// # Create a PIV slot from a named slot
/// $slot = [powershellYK.PIV.PIVSlot]::new("Management")
/// Write-Host "Slot value: $($slot.Value)"
/// 
/// .EXAMPLE
/// # Convert slot to named format
/// $namedSlot = $slot.ToNamedSlot()
/// Write-Host "Named slot: $namedSlot"
/// </summary>

// Imports
using Yubico.YubiKey.Piv;

namespace powershellYK.PIV
{
    // Represents a PIV slot with validation and conversion capabilities
    public readonly struct PIVSlot
    {
        // Internal slot value
        private readonly byte _value;
        private readonly HashSet<byte> _validPIVSlots = new HashSet<byte> { 0x80, 0x81, 0x9b, 0x9a, 0x9c, 0x9d, 0x9e, 0xf9, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95 };

        public PIVSlot(byte value)
        {
            if (!PivSlot.IsValidSlotNumber((byte)value))
            {
                throw new System.ArgumentOutOfRangeException("Value must be a valid PIV slot");
            }
            _value = value;
        }

        // Creates a new PIV slot from an integer value
        public PIVSlot(int value)
        {
            if (value < 0 || value > 0xFF)
            {
                throw new System.ArgumentOutOfRangeException("Value must be between 0 and 255");
            }
            if (!PivSlot.IsValidSlotNumber((byte)value))
            {
                throw new System.ArgumentOutOfRangeException("Value must be a valid PIV slot");
            }
            _value = (byte)value;
        }

        // Creates a new PIV slot from a string value
        public PIVSlot(string value)
        {
            if (String.Equals(value, "Management", StringComparison.OrdinalIgnoreCase))
            {
                _value = 0x9b;
            }
            else if (String.Equals(value, "PIV Authentication", StringComparison.OrdinalIgnoreCase) || String.Equals(value, "Authentication", StringComparison.OrdinalIgnoreCase))
            {
                _value = 0x9a;
            }
            else if (String.Equals(value, "Digital Signature", StringComparison.OrdinalIgnoreCase) || String.Equals(value, "Signature", StringComparison.OrdinalIgnoreCase))
            {
                _value = 0x9c;
            }
            else if (String.Equals(value, "Key Management", StringComparison.OrdinalIgnoreCase))
            {
                _value = 0x9d;
            }
            else if (String.Equals(value, "Card Authentication", StringComparison.OrdinalIgnoreCase) || String.Equals(value, "Card", StringComparison.OrdinalIgnoreCase))
            {
                _value = 0x9e;
            }
            else if (String.Equals(value, "Attestation", StringComparison.OrdinalIgnoreCase))
            {
                _value = 0xf9;
            }
            else
            {
                // Try to see if the value is a hex value
                foreach (var slot in _validPIVSlots)
                {
                    if (String.Equals(value, slot.ToString("X2"), StringComparison.OrdinalIgnoreCase) || String.Equals(value, $"0x{slot.ToString("X2")}", StringComparison.OrdinalIgnoreCase))
                    {
                        _value = slot;
                        return;
                    }
                }
                throw new System.ArgumentOutOfRangeException("Value must be a valid namned PIV slot");
            }
        }

        //Property to get and set the value

        public byte Value
        {
            get { return _value; }
        }

        #region Destinations

        // Returns the slot value as a byte
        public byte ToByte()
        {
            return _value;
        }

        // Returns a string representation of the slot
        public override string ToString()
        {
            return $"0x{_value.ToString("X2")}";
        }

        // Returns the named representation of the slot
        public string ToNamedSlot()
        {
            switch (_value)
            {
                case 0x9b:
                    return "Management";
                case 0x9a:
                    return "PIV Authentication";
                case 0x9c:
                    return "Digital Signature";
                case 0x9d:
                    return "Key Management";
                case 0x9e:
                    return "Card Authentication";
                case 0xf9:
                    return "Attestation";
                case 0x82:
                    return "Retired Key 1";
                case 0x83:
                    return "Retired Key 2";
                case 0x84:
                    return "Retired Key 3";
                case 0x85:
                    return "Retired Key 4";
                case 0x86:
                    return "Retired Key 5";
                case 0x87:
                    return "Retired Key 6";
                case 0x88:
                    return "Retired Key 7";
                case 0x89:
                    return "Retired Key 8";
                case 0x8a:
                    return "Retired Key 9";
                case 0x8b:
                    return "Retired Key 10";
                case 0x8c:
                    return "Retired Key 11";
                case 0x8d:
                    return "Retired Key 12";
                case 0x8e:
                    return "Retired Key 13";
                case 0x8f:
                    return "Retired Key 14";
                case 0x90:
                    return "Retired Key 15";
                case 0x91:
                    return "Retired Key 16";
                case 0x92:
                    return "Retired Key 17";
                case 0x93:
                    return "Retired Key 18";
                case 0x94:
                    return "Retired Key 19";
                case 0x95:
                    return "Retired Key 20";
                default:
                    return _value.ToString();
            }
        }

        #endregion // Destinations

        #region Operators

        // Implicit conversion from byte
        public static implicit operator PIVSlot(byte value)
        {
            return new PIVSlot(value);
        }

        // Implicit conversion from int
        public static implicit operator PIVSlot(int value)
        {
            return new PIVSlot((byte)value);
        }

        // Implicit conversion to byte
        public static implicit operator byte(PIVSlot slot)
        {
            return slot.Value;
        }

        // Implicit conversion to int
        public static implicit operator int(PIVSlot slot)
        {
            return slot.Value;
        }

        // Implicit conversion from string
        public static implicit operator PIVSlot(string value)
        {
            return new PIVSlot(value);
        }

        #endregion // Operators
    }
}
