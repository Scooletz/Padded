using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;
using Padded.Fody;

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
        }
    }
}