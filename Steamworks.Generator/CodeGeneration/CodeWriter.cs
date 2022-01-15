using System.Text;

namespace Steamworks.Generator.CodeGeneration
{
    public class CodeWriter
    {
        private const int IndentCharCount = 4;

        private string Indent => _indentCache ??= new string(' ', IndentCharCount * _indent);

        private readonly StringBuilder _builder;

        private int _indent;
        private int _append;

        private string? _indentCache;

        public CodeWriter()
        {
            _builder = new StringBuilder();
        }

        public void Reset()
        {
            _builder.Clear();

            _indent = 0;
            _append = 0;

            _indentCache = null;
        }

        public CodeWriter Write(char c)
        {
            if (_append > 0)
            {
                _builder.Append(c);
                return this;
            }

            _builder.Append(Indent);
            _builder.Append(c);
            _builder.AppendLine();
            return this;
        }

        public CodeWriter Write(ReadOnlySpan<char> line)
        {
            if (line.IsEmpty)
            {
                if (_append == 0) _builder.AppendLine();
                return this;
            }

            if (_append > 0)
            {
                _builder.Append(line);
                return this;
            }

            _builder.Append(Indent);
            _builder.Append(line);
            _builder.AppendLine();
            return this;
        }

        public CodeWriter WriteLine()
        {
            _builder.AppendLine();
            return this;
        }

        public CodeAppendContext AppendContext()
        {
            return new CodeAppendContext(this);
        }

        public CodeBlockContext BlockContext()
        {
            return new CodeBlockContext(this);
        }

        public void BeginAppend()
        {
            if (_append == 0) _builder.Append(Indent);
            _append++;
        }

        public bool EndAppend()
        {
            _append--;
            if (_append < 0) _append = 0;
            return _append == 0;
        }

        public void BeginBlock(char c = '{')
        {
            Write(c);

            _indent++;
            _indentCache = null;
        }

        public void EndBlock(char c = '}')
        {
            _indentCache = null;

            _indent--;
            if (_indent < 0) _indent = 0;

            Write(c);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}