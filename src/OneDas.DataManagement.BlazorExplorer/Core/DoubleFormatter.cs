using MessagePack;
using MessagePack.Formatters;
using System;

namespace OneDas.DataManagement.BlazorExplorer.Core
{
    public class Test : IFormatterResolver
    {
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (typeof(T) is double)
                return (IMessagePackFormatter<T>)NullDoubleFormatter.Instance;
            else
                return null;
        }
    }

    public class NullDoubleFormatter : IMessagePackFormatter<Double>
    {
        public static IMessagePackFormatter<double> Instance = new NullDoubleFormatter();

        public int Serialize(ref byte[] bytes, int offset, Double value, IFormatterResolver formatterResolver)
        {
            if (double.IsNaN(value))
                return MessagePackBinary.WriteNil(ref bytes, offset);
            else
                return MessagePackBinary.WriteDouble(ref bytes, offset, value);
        }

        public Double Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return MessagePackBinary.ReadDouble(bytes, offset, out readSize);
        }
    }
}
