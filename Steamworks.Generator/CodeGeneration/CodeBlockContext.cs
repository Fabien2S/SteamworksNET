namespace Steamworks.Generator.CodeGeneration;

public readonly ref struct CodeBlockContext
{
    private readonly CodeWriter _writer;

    public CodeBlockContext(CodeWriter writer)
    {
        _writer = writer;

        _writer.BeginBlock();
    }

    public void Dispose()
    {
        _writer.EndBlock();
    }
}