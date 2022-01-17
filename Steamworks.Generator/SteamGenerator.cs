using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Models;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    private readonly CodeWriter _writer;
    private readonly SteamDefinitionModel _model;

    public SteamGenerator(in SteamDefinitionModel model)
    {
        _writer = new CodeWriter();
        _model = model;
    }

    private CodeBlockContext CodeWriterContext()
    {
        _writer.Reset();

        _writer.WriteUsing("Steamworks.Callbacks");
        _writer.WriteUsing("Steamworks.Native");
        _writer.WriteUsing("System.Runtime.InteropServices");
        _writer.WriteLine();

        return _writer.WriteNamespace("Steamworks");
    }
}