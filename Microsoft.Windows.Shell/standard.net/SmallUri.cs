
namespace Standard
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    [DebuggerDisplay("SmallUri: { GetUri() }")]
    internal struct SmallUri : IEquatable<SmallUri>
    {
        private static readonly UTF8Encoding s_Encoder = new UTF8Encoding(false /* do not emit BOM */, true /* throw on error */);
        private readonly byte[] _utf8String;
        private readonly bool _isHttp;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SmallUri(Uri value)
        {
            _isHttp = false;
            _utf8String = null;

            if (value == null)
            {
                return;
            }

            if (!value.IsAbsoluteUri)
            {
                throw new ArgumentException("The parameter is not a valid absolute uri", "value");
            }

            string strValue = value.OriginalString;
            if (strValue.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                _isHttp = true;
                strValue = strValue.Substring(7);
            }

            _utf8String = s_Encoder.GetBytes(strValue);
            Assert.IsNotNull(_utf8String);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SmallUri(string value)
        {
            _isHttp = false;
            _utf8String = null;

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                throw new ArgumentException("The parameter is not a valid uri", "value");
            }
            
            if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                _isHttp = true;
                value = value.Substring(7);
            }
            
            _utf8String = s_Encoder.GetBytes(value);
            Assert.IsNotNull(_utf8String);
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
            // Intentionally hashes similarly to the expanded strings.
            return GetString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            try
            {
                return Equals((SmallUri)obj);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        #endregion

        #region IEquatable<SmallUri> Members

        public bool Equals(SmallUri other)
        {
            if (_utf8String == null)
            {
                return other._utf8String == null;
            }

            if (other._utf8String == null)
            {
                return false;
            }

            if (_isHttp != other._isHttp)
            {
                return false;
            }

            if (_utf8String.Length != other._utf8String.Length)
            {
                return false;
            }

            return Utility.MemCmp(_utf8String, other._utf8String, _utf8String.Length);
        }

        #endregion

        public string GetString()
        {
            if (_utf8String == null)
            {
                return "";
            }
            return GetUri().ToString();
        }

        public Uri GetUri()
        {
            if (_utf8String == null)
            {
                return null;
            }
            return new Uri((_isHttp ? "http://" : "") + s_Encoder.GetString(_utf8String), UriKind.Absolute);
        }

        public static bool operator ==(SmallUri left, SmallUri right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SmallUri left, SmallUri right)
        {
            return !left.Equals(right);
        }
    }
}