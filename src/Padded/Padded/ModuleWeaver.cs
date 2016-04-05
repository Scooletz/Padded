using System;
using System.Linq;
using Mono.Cecil;

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
                
            }
        }

        private static bool HasPaddedAttribute(TypeDefinition type)
        {
            var attrs = type.CustomAttributes;
            return attrs.Any() && attrs.Any(ca => ca.AttributeType.FullName == "Padded.Fody.PaddedAttribute");
        }
    }
}