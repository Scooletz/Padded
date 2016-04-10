using Padded.Fody;

namespace AssemblyToProcess
{
    [Padded]
    public class SimplePaddedClass
    {
        public byte ByteValue;
        public int IntValue;
        public long LongValue;
        public object ObjectValue;
    }
}