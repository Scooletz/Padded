using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Padded.Tests.Utils
{
    public class FieldAddresFinderTests
    {
        public struct Struct1
        {
            public int Int1;
            public long Long2;
            public int Int3;
            public long Long4;
            public object Obj5;
            public Guid Guid6;
        }

        public class Class1
        {
            public int Int1;
            public long Long2;
            public int Int3;
            public long Long4;
            public object Obj5;
            public Guid Guid6;
        }

        [Test]
        public void ExplicitStruct()
        {
            var fields = FieldAddressFinder.GetFieldOffsets(typeof (Struct1));
            Print(fields);
        }

        [Test]
        public void ExplicitClass()
        {
            var fields = FieldAddressFinder.GetFieldOffsets(typeof (Class1));
            Print(fields);
        }

        private static void Print(Dictionary<FieldInfo, int> offsets)
        {
            foreach (var kvp in offsets.OrderBy(kvp => kvp.Value))
            {
                Console.WriteLine($"{kvp.Key.Name}:\t{kvp.Value}");
            }
        }
    }
}