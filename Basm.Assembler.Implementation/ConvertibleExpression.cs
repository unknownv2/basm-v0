using System;

namespace Basm.Assembler.Implementation
{
    internal sealed class ConvertibleExpression : IConvertible
    {
        private readonly IExpressionResolver _resolver;
        private readonly IExpression _expression;

        public ConvertibleExpression(IExpressionResolver resolver, IExpression expression)
        {
            _resolver = resolver;
            _expression = expression;
        }

        public TypeCode GetTypeCode() => TypeCode.Object;
        private T Resolve<T>() => (T)_resolver.Resolve(_expression, typeof(T));
        public object ToType(Type conversionType, IFormatProvider provider) => _resolver.Resolve(_expression, conversionType);
        public bool ToBoolean(IFormatProvider provider) => Resolve<Boolean>();
        public byte ToByte(IFormatProvider provider) => Resolve<Byte>();
        public char ToChar(IFormatProvider provider) => Resolve<Char>();
        public DateTime ToDateTime(IFormatProvider provider) => Resolve<DateTime>();
        public decimal ToDecimal(IFormatProvider provider) => Resolve<Decimal>();
        public double ToDouble(IFormatProvider provider) => Resolve<Double>();
        public short ToInt16(IFormatProvider provider) => Resolve<Int16>();
        public int ToInt32(IFormatProvider provider) => Resolve<Int32>();
        public long ToInt64(IFormatProvider provider) => Resolve<Int64>();
        public sbyte ToSByte(IFormatProvider provider) => Resolve<SByte>();
        public float ToSingle(IFormatProvider provider) => Resolve<Single>();
        public ushort ToUInt16(IFormatProvider provider) => Resolve<UInt16>();
        public uint ToUInt32(IFormatProvider provider) => Resolve<UInt32>();
        public ulong ToUInt64(IFormatProvider provider) => Resolve<UInt64>();
        public string ToString(IFormatProvider provider) => Resolve<String>();
    }
}
