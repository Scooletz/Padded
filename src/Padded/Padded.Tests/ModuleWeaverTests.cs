using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;
using Padded.Fody;
using Padded.Tests.Utils;

namespace Padded.Tests
{
    public class ModuleWeaverTests
    {
        private const int CacheLineSize = 64;

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

        private static void TestType(Type type)
        {
            var offsets = FieldAddressFinder.GetFieldOffsets(type);
            var fieldOrdered = offsets.OrderBy(_ => _.Value).ToArray();
            var last = fieldOrdered.Last().Value;

            var fromBeginningTo =
                fieldOrdered.SkipWhile(_ => _.Key.Name.StartsWith("$padding_")).First().Value;
            var fromEndTo =
                fieldOrdered.OrderByDescending(_ => _.Value)
                    .TakeWhile(_ => _.Key.Name.StartsWith("$padding_"))
                    .Select(kvp => kvp.Value)
                    .Last();

            Assert.LessOrEqual(CacheLineSize, last - fromEndTo);
            Assert.LessOrEqual(CacheLineSize, fromBeginningTo);
        }
    }
}