using System.Text;
using System.Text.Json;
using Steamworks.Generator.Models;

namespace Steamworks.Generator;

internal static class Program
{
    private static int Main()
    {
        Generate("steam_api.json", "Steam");
        Generate("custom_steam_api.json", "Custom");
        return 0;
    }

    private static void Generate(string filePath, string name)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var model = JsonSerializer.Deserialize<SteamDefinitionModel>(fileStream);

        var generator = new SteamGenerator(in model);

        var accessors = generator.GenerateAccessors();
        TrySave(accessors, name, "Accessors");

        var native = generator.GenerateNative();
        TrySave(native, name, "Native");

        var constants = generator.GenerateConstants();
        TrySave(constants, name, "Constants");

        var typeDefs = generator.GenerateTypeDefs();
        TrySave(typeDefs, name, "TypeDefs");

        var enums = generator.GenerateEnums();
        TrySave(enums, name, "Enums");

        var interfaces = generator.GenerateInterfaces();
        TrySave(interfaces, name, "Interfaces");

        var structs = generator.GenerateStructs();
        TrySave(structs, name, "Structs");

        var callbacks = generator.GenerateCallbackStructs();
        TrySave(callbacks, name, "Callbacks");
    }

    private static void TrySave(string text, string @base, string name)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var fileName = string.Join('.', @base, name, "Generated", "cs");
        var path = Path.Join("Generated", fileName);
        File.WriteAllText(path, text, Encoding.UTF8);
    }
}