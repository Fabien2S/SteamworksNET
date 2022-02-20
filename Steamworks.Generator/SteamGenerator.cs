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

        _writer.Write("using Steamworks.Callbacks;");
        _writer.Write("using Steamworks.Native;");
        _writer.Write("using System;");
        _writer.Write("using System.Runtime.InteropServices;");
        _writer.WriteLine();

        return _writer.WriteBlock("namespace Steamworks");
    }
}