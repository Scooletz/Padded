using Padded.Fody;

namespace AssemblyToProcess
{
    [Padded]
    public struct PaddedStruct
    {
        public byte ByteValue;
        public int IntValue;
        public long LongValue;
        public object ObjectValue;
    }
}