using Steamworks.Generator.Models;

namespace Steamworks.Generator.Types;

public static partial class TypeFormatter
{
    public static void FormatConstant(ref ConstantModel constant)
    {
        constant.Type = TypeConverter.ConvertType(constant.Type);
        constant.Value = FormatValue(constant.Value);

        constant.Value = constant.Name switch
        {
            // [...]UL
            // int cannot be cast to SteamItemInstanceID_t (ulong)
            "k_SteamItemInstanceIDInvalid" => "( SteamItemInstanceID_t ) ~0UL",

            // unchecked((int) [...])
            // Overflowing is only possible inside unchecked context
            "HSERVERQUERY_INVALID" => "unchecked((int) 0xffffffff)",

            // uint32 -> uint, 16U -> 16, 8U -> 8
            // uint32 doesn't exists in C#
            "k_SteamDatagramPOPID_dev" => "( ( uint ) 'd' << 16 ) | ( ( uint ) 'e' << 8 ) | ( uint ) 'v'",

            _ => constant.Value,
        };
    }
}