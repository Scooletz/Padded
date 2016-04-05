using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Padded.Fody;

namespace Padded.Tests
{
    public class FieldAddresFinderTests
    {
        [StructLayout(LayoutKind.Explicit, Size = 24)]
        public class Struct1
        {
            [FieldOffset(8)] public int Int;

            [FieldOffset(0)] public long Long;
        }

        [Test]
        public void ExplicitStruct()
        {
            var fields = FieldAddressFinder.GetFieldOffsets(typeof (Struct1));
        }
    }
}