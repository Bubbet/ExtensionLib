using System.Collections.Generic;
using JetBrains.Annotations;
using Mono.Cecil;

namespace Com.Dipolecat.ExtensionLib.Preload
{
    public static class Patcher
    {
        [UsedImplicitly]
        public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp.dll"];

        [UsedImplicitly]
        public static void Patch(AssemblyDefinition assembly){
            AtmosphericsPatcher.Patch(assembly);
            ReagentPatcher.Patch(assembly);
        }
    }
}