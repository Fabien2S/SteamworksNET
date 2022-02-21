using Steamworks.Generator.CodeGeneration;

namespace Steamworks.Generator.Extensions;

public static partial class SteamCodeWriterExtensions
{
    public static void WriteOperatorEquals(this CodeWriter writer, string type, string flatName)
    {
        writer.Write($"public override bool Equals(object obj) => obj is {type} x && SteamNative.{flatName}(ref this, &x);");
        writer.WriteLine();
        writer.Write($"public static bool operator ==({type} x, {type} y) => SteamNative.{flatName}(ref x, &y);");
        writer.WriteLine();
        writer.Write($"public static bool operator !=({type} x, {type} y) => !SteamNative.{flatName}(ref x, &y);");
        writer.WriteLine();
    }
}