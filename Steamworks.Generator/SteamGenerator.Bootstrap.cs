using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Models;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    private readonly SteamDefinitionModel _model;
    private readonly string _dllName;
    private readonly string _pack;

    private readonly CodeWriter _writer;

    public SteamGenerator(in SteamDefinitionModel model, string dllName, string pack)
    {
        _model = model;
        _pack = pack;
        _dllName = dllName;

        _writer = new CodeWriter();
    }

    private void Prepare()
    {
        _writer.Reset();

        _writer.WriteUsing("Steamworks.Native");
        _writer.WriteUsing("System.Runtime.InteropServices");
        _writer.WriteLine();

        _writer.WriteNamespace("Steamworks");
        _writer.WriteLine();
    }

    public string GenerateBootstrap()
    {
        Prepare();

        using (_writer.WriteClass("SteamAPI", "public static"))
        {
            _writer.WriteDllImportAttribute(_dllName, "SteamAPI_Init");
            _writer.WriteMarshalAsAttribute(UnmanagedType.I1, "return");
            _writer.Write("private static extern bool SteamAPI_Init();");
            _writer.WriteLine();

            _writer.WriteDllImportAttribute(_dllName, "SteamAPI_Shutdown");
            _writer.Write("private static extern void SteamAPI_Shutdown();");
            _writer.WriteLine();

            _writer.WriteDllImportAttribute(_dllName, "SteamAPI_RestartAppIfNecessary");
            _writer.WriteMarshalAsAttribute(UnmanagedType.I1, "return");
            _writer.Write("private static extern bool SteamAPI_RestartAppIfNecessary( uint unOwnAppID );");
            _writer.WriteLine();

            _writer.Write("public static bool Init() => SteamAPI_Init();");
            _writer.Write("public static void Shutdown() => SteamAPI_Shutdown();");
            _writer.Write(
                "public static bool RestartAppIfNecessary(AppId_t appId) => SteamAPI_RestartAppIfNecessary(appId);");
        }

        return _writer.ToString();
    }
}