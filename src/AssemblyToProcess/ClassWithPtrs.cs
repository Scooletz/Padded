using System;

namespace AssemblyToProcess
{
    public sealed class ClassWithPtrs
    {
        private IDisposable d;
        private long _headCache;
        private StructWithPtr _head;
        private StructWithPtr tail;
        private int _mask;
    }
}