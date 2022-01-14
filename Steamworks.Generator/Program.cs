using System.Text;
using System.Text.Json;
using Steamworks.Generator.Models;

namespace Steamworks.Generator;

internal static class Program
{
    private static int Main()
    {
        const string dllName = "steam_api64";
        const string dllPack = "4";

        Generate("steam_api.json", true, dllName, dllPack);
        Generate("custom_steam_api.json", false, dllName, dllPack);
        return 0;
    }

    private static void Generate(string filePath, bool isMain, string dllName, string dllPack)
    {
        var baseName = isMain ? "Steamworks" : "Custom";

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var model = JsonSerializer.Deserialize<SteamDefinitionModel>(fileStream);

        var generator = new SteamGenerator(in model, dllName, dllPack);

        if (isMain)
        {
            var bootstrap = generator.GenerateBootstrap();
            TrySave(bootstrap, baseName, "Bootstrap");
        }

        var constants = generator.GenerateConstants();
        TrySave(constants, baseName, "Constants");

        var typeDefs = generator.GenerateTypeDefs();
        TrySave(typeDefs, baseName, "TypeDefs");

        var enums = generator.GenerateEnums();
        TrySave(enums, baseName, "Enums");

        var interfaces = generator.GenerateInterfaces();
        TrySave(interfaces, baseName, "Interfaces");

        var structs = generator.GenerateStructs();
        TrySave(structs, baseName, "Structs");

        var callbacks = generator.GenerateCallbackStructs();
        TrySave(callbacks, baseName, "Callbacks");
    }

    private static void TrySave(string text, string @base, string name)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var fileName = string.Concat(@base, ".", name, ".cs");
        var path = Path.Join("Generated", fileName);
        File.WriteAllText(path, text, Encoding.UTF8);
    }
}