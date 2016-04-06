using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Padded.Fody
{
    public class ModuleWeaver
    {
        public const string PaddingFieldPrefix = "$padding_";

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
                if (paddedType.IsInterface)
                {
                    throw new Exception("Padded.Fody cannot add padding to the interface");
                }

                if (paddedType.IsAbstract)
                {
                    throw new Exception("Padded.Fody should be used only on concrete classes");
                }

                PadType(paddedType);
            }
        }

        public static void PadType(TypeDefinition t)
        {
            t.Attributes |= TypeAttributes.SequentialLayout;
            var g = t.Module.ImportReference(typeof (Guid));

            // 4x16 at the beginning
            t.Fields.Insert(0, GetField(g, 1));
            t.Fields.Insert(1, GetField(g, 2));
            t.Fields.Insert(2, GetField(g, 3));
            t.Fields.Insert(3, GetField(g, 4));

            // 4x16 at the end
            t.Fields.Add(GetField(g, 5));
            t.Fields.Add(GetField(g, 6));
            t.Fields.Add(GetField(g, 7));
            t.Fields.Add(GetField(g, 8));
        }

        private static FieldDefinition GetField(TypeReference guidType, int i)
        {
            return new FieldDefinition($"{PaddingFieldPrefix}{i}", FieldAttributes.Private, guidType);
        }

        private static bool HasPaddedAttribute(TypeDefinition type)
        {
            var attrs = type.CustomAttributes;
            return attrs.Any() && attrs.Any(ca => ca.AttributeType.FullName == "Padded.Fody.PaddedAttribute");
        }
    }
}