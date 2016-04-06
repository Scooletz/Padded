using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using NUnit.Framework;
using Padded.Fody;
using Padded.Tests.Utils;

namespace Padded.Tests
{
    public class ModuleWeaverTests
    {
        [Test]
        public void Test()
        {
            var beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)

            beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

            var afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "Padded.dll");
            File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

            var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath);
            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
            };

            weavingTask.Execute();
            moduleDefinition.Write(afterAssemblyPath);

            var assembly = Assembly.LoadFile(afterAssemblyPath);

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.Name.StartsWith("Padded") && type.Name != "PaddedAttribute")
                {
                    TestType(type);
                }
            }
        }

        private void TestType(Type type)
        {
            var offsets = FieldAddressFinder.GetFieldOffsets(type);
            var fieldOrdered = offsets.OrderBy(kvp => kvp.Value).ToArray();

            for (var i = 0; i < 4; i++)
            {
                var kvp = fieldOrdered[i];
                Assert.True(kvp.Key.Name.StartsWith(ModuleWeaver.PaddingFieldPrefix));
                Assert.AreEqual(typeof (Guid), kvp.Key.FieldType);
                Assert.AreEqual(i*Marshal.SizeOf<Guid>(), kvp.Key);
            }

            for (var i = fieldOrdered.Length - 4; i < 4; i++)
            {
                var kvp = fieldOrdered[i];
                Assert.True(kvp.Key.Name.StartsWith(ModuleWeaver.PaddingFieldPrefix));
                Assert.AreEqual(typeof (Guid), kvp.Key.FieldType);
            }
        }
    }
}