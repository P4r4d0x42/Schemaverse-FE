// Guids.cs
// MUST match guids.h
using System;

namespace Npgsql.Provider
{
    static class GuidList
    {
        public const string guidNpgsql_DesignerPkgString = "8ed13e30-13c2-4fd9-9605-173441f9f55b";
        public const string guidNpgsql_DesignerCmdSetString = "8435fb52-5642-464c-9acd-d1ddcff8d1d3";
        public const string guidToolWindowPersistanceString = "e369062e-ead6-4ef4-b942-0500f1187d28";
        public const string guidNpgsql_DesignerEditorFactoryString = "8c88e993-7b10-4c2a-a8ee-c50c9a098b33";

        public static readonly Guid guidNpgsql_DesignerCmdSet = new Guid(guidNpgsql_DesignerCmdSetString);
        public static readonly Guid guidNpgsql_DesignerEditorFactory = new Guid(guidNpgsql_DesignerEditorFactoryString);
    };
}