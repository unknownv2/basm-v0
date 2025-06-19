using Basm.Assembler;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Basm.CheatEngine
{
    public class ValueNode : IValueNode
    {
        private static NumberStyles Base10NumberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        private static readonly Regex 
            ByteArrayRegex = new Regex("( *[0-9a-fA-F]{2} *)+"),
            WildcardfByteArrayRegex = new Regex("( *[0-9a-fA-F?*]{2} *)+");

        private readonly string _value;
        private readonly NumberStyles _numberStyles;

        public ValueNode(string value, bool defaultToHex = false)
        {
            if (value[0] == '$')
            {
                _numberStyles = NumberStyles.AllowHexSpecifier;
                value = value.Remove(0, 1);
            }
            else if (value[0] == '#')
            {
                _numberStyles = Base10NumberStyle;
                value = value.Remove(0, 1);
            }
            else
            {
                _numberStyles = defaultToHex
                    ? NumberStyles.AllowHexSpecifier
                    : Base10NumberStyle;
            }

            _value = value;
        }

        public object GetValue(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return _value;
                case TypeCode.Boolean:
                    return ToBool();
                case TypeCode.Char:
                    return _value[0];
                case TypeCode.Byte:
                    return Byte.Parse(_value, _numberStyles);
                case TypeCode.SByte:
                    return SByte.Parse(_value, _numberStyles);
                case TypeCode.Int16:
                    return Int16.Parse(_value, _numberStyles);
                case TypeCode.UInt16:
                    return UInt16.Parse(_value, _numberStyles);
                case TypeCode.Int32:
                    return Int32.Parse(_value, _numberStyles);
                case TypeCode.UInt32:
                    return UInt32.Parse(_value, _numberStyles);
                case TypeCode.Int64:
                    return Int64.Parse(_value, _numberStyles);
                case TypeCode.UInt64:
                    return UInt64.Parse(_value, _numberStyles);
                case TypeCode.Single:
                    return Single.Parse(_value, _numberStyles);
                case TypeCode.Double:
                    return Double.Parse(_value, _numberStyles);
                case TypeCode.Decimal:
                    return Decimal.Parse(_value, _numberStyles);
            }

            if (type == typeof(IntPtr))
            {
                return new IntPtr(Int64.Parse(_value, _numberStyles));
            }

            if (type == typeof(UIntPtr))
            {
                return new UIntPtr(UInt64.Parse(_value, _numberStyles));
            }

            if (type == typeof(Byte[]))
            {
                return ToByteArray();
            }

            if (type == typeof(Byte?[]))
            {
                return ToNullableByteArray();
            }

            return null;
        }

        private bool ToBool()
        {
            if (_value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (_value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (decimal.TryParse(_value, _numberStyles, CultureInfo.InvariantCulture, out decimal value))
            {
                return value != 0;
            }

            return !string.IsNullOrWhiteSpace(_value);
        }

        private byte[] ToByteArray()
        {
            var value = _value.Replace(" ", "");
            var bytes = new byte[value.Length / 2];
            for (var x = 0; x < bytes.Length; x++)
            {
                bytes[x] = byte.Parse(value.Substring(x * 2, 2), NumberStyles.AllowHexSpecifier);
            }
            return bytes;
        }

        private byte?[] ToNullableByteArray()
        {
            var value = _value.Replace(" ", "");
            var bytes = new byte?[value.Length / 2];
            for (var x = 0; x < bytes.Length; x++)
            {
                var byteStr = value.Substring(x * 2, 2);
                bytes[x] = byteStr == "??" || byteStr == "**"
                    ? null
                    : (byte?)byte.Parse(byteStr, NumberStyles.AllowHexSpecifier);
            }
            return bytes;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
