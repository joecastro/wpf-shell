
namespace Standard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    [System.Diagnostics.DebuggerDisplay("SmallString: { GetString() }")]
    internal struct SmallString : IEquatable<SmallString>, IComparable<SmallString>
    {
        [Flags]
        private enum _SmallFlags : byte
        {
            None = 0,
            IsInt64 = 1,
            HasHashCode = 2,
            Reserved = 4,
        }

        private static readonly System.Text.UTF8Encoding s_Encoder = new System.Text.UTF8Encoding(false /* do not emit BOM */, true /* throw on error */);
        private readonly byte[] _encodedBytes;
        private _SmallFlags _flags;
        private int _cachedHashCode;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SmallString(string value, bool precacheHashCode = false)
        {
            _flags = _SmallFlags.None;
            _cachedHashCode = 0;

            if (!string.IsNullOrEmpty(value))
            {
                if (precacheHashCode)
                {
                    _flags |= _SmallFlags.HasHashCode;
                    _cachedHashCode = value.GetHashCode();
                }

                long numValue;
                if (long.TryParse(value, System.Globalization.NumberStyles.None, null, out numValue))
                {
                    _flags |= _SmallFlags.IsInt64;
                    _encodedBytes = BitConverter.GetBytes(numValue);

                    // It's possible that this doesn't round trip with full fidelity.
                    // If this assert ever gets hit, consider adding an overload that opts
                    // out of this optimization. 
                    // (Note that the parameters are not evaluated on retail builds)
                    Assert.AreEqual(this.GetString(), value);
                    
                    return;
                }

                _encodedBytes = s_Encoder.GetBytes(value);
                Assert.IsNotNull(_encodedBytes);
            }
            else
            {
                _encodedBytes = null;
            }
        }

        private bool _IsInt64
        {
            get { return (_flags & _SmallFlags.IsInt64) == _SmallFlags.IsInt64; }
        }

        private bool _HasCachedHashCode
        {
            get { return (_flags & _SmallFlags.HasHashCode) == _SmallFlags.HasHashCode; }
        }

        #region Object Overrides

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GetString")]
        public override string ToString()
        {
            Assert.Fail();
            throw new NotSupportedException("This exception exists to prevent accidental performance penalties.  Call GetString() instead.");
        }

        public override int GetHashCode()
        {
            if (_encodedBytes == null)
            {
                return 0;
            }

            if (!_HasCachedHashCode)
            {
                // Intentionally hashes similarly to the expanded strings.
                _cachedHashCode = GetString().GetHashCode();
                _flags |= _SmallFlags.HasHashCode;
            }

            return _cachedHashCode;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return Equals((SmallString)obj);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        #endregion

        #region IEquatable<SmallString> Members

        public bool Equals(SmallString other)
        {
            if (_encodedBytes == null)
            {
                return other._encodedBytes == null;
            }

            if (other._encodedBytes == null)
            {
                return false;
            }

            if (_encodedBytes.Length != other._encodedBytes.Length)
            {
                return false;
            }

            // If only one is a number, then they're not equal.
            if (((_flags ^ other._flags) & _SmallFlags.IsInt64) == _SmallFlags.IsInt64)
            {
                return false;
            }

            if (_HasCachedHashCode && other._HasCachedHashCode)
            {
                if (_cachedHashCode != other._cachedHashCode)
                {
                    return false;
                }
            }

            if (_IsInt64)
            {
                return BitConverter.ToInt64(_encodedBytes, 0) == BitConverter.ToInt64(other._encodedBytes, 0);
            }

            // Note that this is doing a literal binary comparison of the two strings.
            // It's possible for two real strings to compare equally even though they
            // can be encoded in different ways with UTF8.
            return Utility.MemCmp(_encodedBytes, other._encodedBytes, _encodedBytes.Length);
        }

        #endregion

        public string GetString()
        {
            if (_encodedBytes == null)
            {
                return "";
            }

            if (_IsInt64)
            {
                return BitConverter.ToInt64(_encodedBytes, 0).ToString(CultureInfo.InvariantCulture);
            }

            return s_Encoder.GetString(_encodedBytes);
        }

        public static bool operator==(SmallString left, SmallString right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(SmallString left, SmallString right)
        {
            return !left.Equals(right);
        }

        #region IComparable<SmallString> Members

        public int CompareTo(SmallString other)
        {
            // If either of the strings contains multibyte characters 
            // then we can't do a strictly bitwise comparison.
            // We can look for a signaled high-bit in the byte to detect this.
            // Opportunistically, we're going to assume that the strings are
            // ASCII compatible until we find out they aren't.

            if (_encodedBytes == null)
            {
                if (other._encodedBytes == null)
                {
                    return 0;
                }
                Assert.AreNotEqual(0, other._encodedBytes.Length);
                return -1;
            }
            else if (other._encodedBytes == null)
            {
                Assert.AreNotEqual(0, _encodedBytes.Length);
                return 1;
            }

            bool? isThisStringShorter = null;
            int cb = _encodedBytes.Length;
            int cbDiffernce = other._encodedBytes.Length - cb;

            if (cbDiffernce < 0)
            {
                isThisStringShorter = false;
                cb = other._encodedBytes.Length; 
            }
            else if (cbDiffernce > 0)
            {
                isThisStringShorter = true;
            }

            for (int i = 0; i < cb; ++i)
            {
                bool isEitherHighBitSet = ((_encodedBytes[i] | other._encodedBytes[i]) & 0x80) != 0;
                // If the byte array contains multibyte characters
                // we need to do a real string comparison.
                if (isEitherHighBitSet)
                {
                    string left = this.GetString();
                    string right = other.GetString();

                    return string.Compare(left, right, StringComparison.Ordinal);
                }

                if (_encodedBytes[i] != other._encodedBytes[i])
                {
                    return _encodedBytes[i] - other._encodedBytes[i];
                }
            }

            if (isThisStringShorter == null)
            {
                return 0;
            }

            if (isThisStringShorter == false)
            {
                return -1;
            }

            return 1;
        }

        #endregion
    }
}