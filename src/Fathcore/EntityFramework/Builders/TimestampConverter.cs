using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fathcore.EntityFramework.Builders
{
    public sealed class TimestampConverter : ValueConverter<byte[], string>
    {
        public TimestampConverter() : base(
            v => v == null ? null : ToString(v),
            v => v == null ? null : ToByte(v))
        { }

        private static byte[] ToByte(string v) =>
            v.Select(c => (byte)c).ToArray();

        private static string ToString(byte[] v) =>
            new string(v.Select(b => (char)b).ToArray());
    }
}
