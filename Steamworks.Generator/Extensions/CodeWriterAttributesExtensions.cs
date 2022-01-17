using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;

namespace Steamworks.Generator.Extensions;

public static class CodeWriterAttributesExtensions
{
    public static void WriteStructLayoutAttribute(this CodeWriter writer, LayoutKind layoutKind)
    {
        using (writer.AppendContext())
        {
            writer
                .Write("[StructLayout(")
                .Write("LayoutKind.").Write(Enum.GetName(layoutKind)).Write(", ")
                .Write("Pack = SteamPlatform.LibraryPack")
                .Write(")]");
        }
    }

    public static void WriteDllImportAttribute(this CodeWriter writer, string entryPoint)
    {
        using (writer.AppendContext())
        {
            writer
                .Write("[DllImport(SteamPlatform.LibraryName").Write(", ")
                .Write("EntryPoint = \"").Write(entryPoint).Write("\", ")
                .Write("CallingConvention = SteamPlatform.CallingConvention")
                .Write(")]");
        }
    }

    public static void WriteMarshalAsAttribute(this CodeWriter writer, UnmanagedType unmanagedType,
        string? prefix = null
    )
    {
        using (writer.AppendContext())
        {
            writer
                .Write("[")
                .Write(string.IsNullOrEmpty(prefix) ? ReadOnlySpan<char>.Empty : prefix + ": ")
                .Write("MarshalAs(UnmanagedType.").Write(Enum.GetName(unmanagedType)).Write(')')
                .Write("]");
        }
    }

    public static void WriteUnmanagedFunctionPointerAttribute(this CodeWriter writer)
    {
        writer.Write("[UnmanagedFunctionPointer(SteamPlatform.CallingConvention)]");
    }
}