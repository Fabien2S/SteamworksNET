using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Models;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    private readonly SteamDefinitionModel _model;
    private readonly string _dllName;
    private readonly string _dllPack;

    private readonly CodeWriter _writer;

    public SteamGenerator(in SteamDefinitionModel model, string dllName, string dllPack)
    {
        _model = model;
        _dllPack = dllPack;
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
}