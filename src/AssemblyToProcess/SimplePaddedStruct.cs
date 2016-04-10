using Padded.Fody;

namespace AssemblyToProcess
{
    [Padded]
    public struct SimplePaddedStruct
    {
        public byte ByteValue;
        public int IntValue;
        public long LongValue;
        public object ObjectValue;
    }
}