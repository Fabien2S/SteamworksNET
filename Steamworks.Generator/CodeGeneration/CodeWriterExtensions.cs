namespace Steamworks.Generator.CodeGeneration;

public static class CodeWriterExtensions
{
    public static void WriteUsing(this CodeWriter writer, ReadOnlySpan<char> @namespace)
    {
        using (writer.AppendContext())
        {
            writer.Write("using ");
            writer.Write(@namespace);
            writer.Write(';');
        }
    }

    public static CodeBlockContext WriteNamespace(this CodeWriter writer, ReadOnlySpan<char> @namespace)
    {
        using (writer.AppendContext())
        {
            writer
                .Write("namespace ")
                .Write(@namespace);
        }

        return writer.BlockContext();
    }

    public static CodeBlockContext WriteClass(this CodeWriter writer, ReadOnlySpan<char> className,
        string modifiers = "public")
    {
        using (writer.AppendContext())
        {
            writer.Write(modifiers);
            writer.Write(" class ");
            writer.Write(className);
        }

        return writer.BlockContext();
    }

    public static CodeBlockContext WriteInterface(this CodeWriter writer, ReadOnlySpan<char> className,
        string modifiers = "public")
    {
        using (writer.AppendContext())
        {
            writer.Write(modifiers);
            writer.Write(" interface ");
            writer.Write(className);
        }

        return writer.BlockContext();
    }

    public static CodeBlockContext WriteStruct(this CodeWriter writer, ReadOnlySpan<char> className,
        string modifiers = "public", string? suffix = null)
    {
        using (writer.AppendContext())
        {
            writer.Write(modifiers);
            writer.Write(" struct ");
            writer.Write(className);

            if (!string.IsNullOrEmpty(suffix))
                writer.Write(' ').Write(suffix);
        }

        return writer.BlockContext();
    }
}