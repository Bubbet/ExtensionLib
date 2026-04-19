using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Com.Dipolecat.ExtensionLib.Preload
{
    public static class ReagentPatcher
    {
        public static void Patch(AssemblyDefinition assembly){
            var reagentType = assembly.MainModule.GetType("Reagents.Reagent");
            Debug.Assert(reagentType!=null,"Could not find Reagent type");
            reagentType.Fields.Add(new FieldDefinition("_extensionlib_reagentid", FieldAttributes.Public, assembly.MainModule.TypeSystem.UInt32));
        }
    }
}