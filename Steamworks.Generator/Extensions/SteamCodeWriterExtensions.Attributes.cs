using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;

namespace Steamworks.Generator.Extensions;

public static partial class SteamCodeWriterExtensions
{
    public static void WriteStructLayoutAttribute(this CodeWriter writer, LayoutKind layoutKind)
    {
        var layoutKindName = Enum.GetName(layoutKind);
        writer.Write($"[StructLayout(LayoutKind.{layoutKindName}, Pack = SteamPlatform.LibraryPack)]");
    }

    public static void WriteDllImportAttribute(this CodeWriter writer, string entryPoint)
    {
        writer.Write($"[DllImport(SteamPlatform.LibraryName, EntryPoint = \"{entryPoint}\", CallingConvention = SteamPlatform.CallingConvention)]");
    }

    public static void WriteMarshalAsAttribute(this CodeWriter writer, UnmanagedType unmanagedType)
    {
        var unmanagedTypeName = Enum.GetName(unmanagedType);
        writer.Write($"[MarshalAs(UnmanagedType.{unmanagedTypeName})]");
    }

    public static void WriteUnmanagedFunctionPointerAttribute(this CodeWriter writer)
    {
        writer.Write("[UnmanagedFunctionPointer(SteamPlatform.CallingConvention)]");
    }
}