using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Padded.Fody
{
    public class ModuleWeaver
    {
        public Action<string> LogDebug { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public Action<string> LogError { get; set; }

        public IAssemblyResolver AssemblyResolver { get; set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        public ModuleWeaver()
        {
            LogDebug = m => { };
            LogInfo = m => { };
            LogWarning = m => { };
            LogError = m => { };
        }

        public void Execute()
        {
            foreach (var paddedType in ModuleDefinition.Types.Where(HasPaddedAttribute))
            {
                PadType(paddedType);
            }
        }

        public static void PadType(TypeDefinition t)
        {
            var corlib = ModuleDefinition.ReadModule(typeof (object).Module.FullyQualifiedName);
            var types = corlib.Types;
            var structLayoutAttribute =
                types.Single(type => type.FullName == "System.Runtime.InteropServices.StructLayoutAttribute");
            var ctor =
                structLayoutAttribute.GetConstructors()
                    .Single(md => md.Parameters.Count == 1 && md.Parameters[0].ParameterType.Name == "Int16");
            var importedCtor = t.Module.ImportReference(ctor);
            var guidType = t.Module.ImportReference(typeof (Guid));
            var ca = new CustomAttribute(importedCtor, new byte[] {0, 0});

            t.Resolve().CustomAttributes.Add(ca);

            // 4x16 at the beginning
            t.Fields.Insert(0, new FieldDefinition("$1", FieldAttributes.Private, guidType));
            t.Fields.Insert(1, new FieldDefinition("$2", FieldAttributes.Private, guidType));
            t.Fields.Insert(2, new FieldDefinition("$3", FieldAttributes.Private, guidType));
            t.Fields.Insert(3, new FieldDefinition("$4", FieldAttributes.Private, guidType));

            // 4x16 at the end
            t.Fields.Add(new FieldDefinition("$5", FieldAttributes.Private, guidType));
            t.Fields.Add(new FieldDefinition("$6", FieldAttributes.Private, guidType));
            t.Fields.Add(new FieldDefinition("$7", FieldAttributes.Private, guidType));
            t.Fields.Add(new FieldDefinition("$8", FieldAttributes.Private, guidType));
        }

        private static bool HasPaddedAttribute(TypeDefinition type)
        {
            var attrs = type.CustomAttributes;
            return attrs.Any() && attrs.Any(ca => ca.AttributeType.FullName == "Padded.Fody.PaddedAttribute");
        }
    }
}