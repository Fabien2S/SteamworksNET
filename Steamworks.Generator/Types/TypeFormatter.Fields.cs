using Steamworks.Generator.Models;

namespace Steamworks.Generator.Types;

public static partial class TypeFormatter
{
    public static void FormatField(ref FieldModel field)
    {
        field.Type = TypeConverter.ConvertType(field.Type);
        field.CustomAttribute = FormatFieldAttribute(in field);
    }

    private static string? FormatFieldAttribute(in FieldModel field)
    {
        return field.Type switch
        {
            "bool" => "[MarshalAs(UnmanagedType.I1)]",
            "string" => "[MarshalAs(UnmanagedType.LPUTF8Str)]",
            _ => field.CustomAttribute
        };
    }
}