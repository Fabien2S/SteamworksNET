using Steamworks.Generator.Models;

namespace Steamworks.Generator.Types;

public static partial class TypeFormatter
{
    public static bool TryFormatField(ref FieldModel field)
    {
        field.Type = TypeConverter.ConvertType(field.Type);
        field.CustomAttribute = FormatFieldAttribute(in field);

        // SteamInputActionEvent_t.AnalogAction_t is not included in steam_api.json 
        if (field.Type.Equals("SteamInputActionEvent_t.AnalogAction_t", StringComparison.Ordinal))
            return false;

        return true;
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