using System.Runtime.InteropServices;

namespace Steamworks.Generator.CodeGeneration;

public static class CodeWriterNativeExtensions
{
    public static void WriteStructLayoutAttribute(this CodeWriter writer, LayoutKind layoutKind, string pack)
    {
        using (writer.AppendContext())
        {
            writer
                .Write("[StructLayout(")
                .Write("LayoutKind.").Write(Enum.GetName(layoutKind)).Write(", ")
                .Write("Pack = ").Write(pack)
                .Write(")]");
        }
    }

    public static void WriteDllImportAttribute(this CodeWriter writer, string dllName, string entryPoint,
        CallingConvention callingConvention = CallingConvention.Cdecl
    )
    {
        using (writer.AppendContext())
        {
            writer
                .Write("[DllImport(\"")
                .Write(dllName).Write("\", ")
                .Write("EntryPoint = \"").Write(entryPoint).Write("\", ")
                .Write("CallingConvention = CallingConvention.").Write(Enum.GetName(callingConvention))
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

    public static void WriteUnmanagedFunctionPointerAttribute(this CodeWriter writer,
        CallingConvention callingConvention = CallingConvention.Cdecl
    )
    {
        using (writer.AppendContext())
        {
            writer
                .Write("[UnmanagedFunctionPointer(")
                .Write("CallingConvention.").Write(Enum.GetName(callingConvention))
                .Write(")]");
        }
    }
}