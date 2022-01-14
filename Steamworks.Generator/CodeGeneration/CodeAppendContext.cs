namespace Steamworks.Generator.CodeGeneration;

public readonly ref struct CodeAppendContext
{
    private readonly CodeWriter _writer;

    public CodeAppendContext(CodeWriter writer)
    {
        _writer = writer;
        _writer.BeginAppend();
    }

    public void Dispose()
    {
        _writer.EndAppend();
        _writer.WriteLine();
    }
}