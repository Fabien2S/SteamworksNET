using Steamworks.Generator.Models;

namespace Steamworks.Generator.Types;

public static class TypePredicate
{
    public static bool ShouldIncludeCallbackStruct(in CallbackStructModel callbackStruct)
    {
        return callbackStruct.Name switch
        {
            // PS3TrophiesInstalled_t cause conflict with 
            "PS3TrophiesInstalled_t" => false,

            _ => true
        };
    }

    public static bool ShouldIncludeField(in FieldModel field)
    {
        return field.Type switch
        {
            // SteamInputActionEvent_t::AnalogAction_t is not included in steam_api.json 
            "SteamInputActionEvent_t::AnalogAction_t" => false,

            _ => true
        };
    }

    public static bool ShouldIncludeMethod(in MethodModel method)
    {
        return method.Name switch
        {
            "<" => false,
            _ => true
        };
    }
}