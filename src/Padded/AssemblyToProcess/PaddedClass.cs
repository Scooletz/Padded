using Padded.Fody;

namespace AssemblyToProcess
{
    [Padded]
    public class PaddedClass
    {
        public byte ByteValue;
        public int IntValue;
        public long LongValue;
        public object ObjectValue;
    }
}