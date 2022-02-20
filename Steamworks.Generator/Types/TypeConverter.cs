namespace Steamworks.Generator.Types;

public static class TypeConverter
{
    public static string ConvertType(string type)
    {
        // 'Class::Subclass' -> 'Class.Subclass'
        type = type.Replace("::", ".", StringComparison.Ordinal);

        // '[...]const[...]' -> '[...]'
        var constIndex = type.IndexOf("const", StringComparison.Ordinal);
        if (constIndex != -1)
        {
            type = type[..constIndex].TrimEnd() + type[(constIndex + 5)..].TrimStart();
        }

        // (*)(Type *) -> Type*
        const string ptrPrefix = "void (*)(";
        const string ptrSuffix = " *)";
        if (type.StartsWith(ptrPrefix, StringComparison.Ordinal) && type.EndsWith(ptrSuffix, StringComparison.Ordinal))
        {
            // TODO Void is the return type
            // TODO Delegate
            return "IntPtr";
        }

        // TODO required?
        // 'ISteam[...]*' -> 'IntPtr'
        if (type.StartsWith("ISteam", StringComparison.Ordinal) && type.EndsWith('*'))
            return "IntPtr";

        // 'char *'
        if (type.Equals("char *", StringComparison.Ordinal))
            return "string";

        // '[...] &' or '[...] *' -> '[...]*'
        if (type.EndsWith(" &", StringComparison.Ordinal) || type.EndsWith(" *", StringComparison.Ordinal))
            return ConvertUnmanagedType(type[..^2]) + '*';

        // '[...] *' or '[...] **' or '[...] ***' and so on -> '[...]*'
        if (type.EndsWith('*'))
        {
            var i = 0;
            while (i < type.Length && type[^(i + 1)] == '*') i++;
            return ConvertUnmanagedType(type[..^i].TrimEnd()) + new string('*', i);
        }

        return ConvertUnmanagedType(type);
    }

    private static string ConvertUnmanagedType(string type)
    {
        return type switch
        {
            "unsigned char" => "byte",
            "signed char" => "sbyte",
            "unsigned short" => "ushort",
            "unsigned int" => "uint",
            "long long" => "long",
            "unsigned long long" => "ulong",

            "uint8" => "byte",
            "int8" => "sbyte",
            "int16" => "short",
            "uint16" => "ushort",

            "int32" => "int",
            "uint32" => "uint",
            "int32_t" => "int",
            "uint32_t" => "uint",

            "int64" => "long",
            "uint64" => "ulong",
            "lint64" => "long",
            "ulint64" => "ulong",
            "intp" => "long",
            "uintp" => "ulong",
            "int64_t" => "long",
            "uint64_t" => "ulong",

            "intptr_t" => "IntPtr",
            "size_t" => "UIntPtr",

            _ => type
        };
    }
}