using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;
using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateTypeDefs()
    {
        if (_model.TypeDefs == null)
            return string.Empty;

        Prepare();

        foreach (var typeDefinition in _model.TypeDefs)
        {
            var name = typeDefinition.Name;

            var formattedName = TypeConverter.ConvertType(typeDefinition.Name);
            if (!string.Equals(name, formattedName, StringComparison.Ordinal)) continue;

            if (SteamConverter.IsFixedSizeArrayType(typeDefinition.Type, out var fixedType, out var fixedSize))
                GenerateTypeDefFixedSize(fixedType, name, fixedSize);
            else if (SteamConverter.IsDelegateType(typeDefinition.Type, out var delegateParameters))
                GenerateTypeDefDelegate(delegateParameters, name);
            else
            {
                var type = TypeConverter.ConvertType(typeDefinition.Type);
                GenerateTypeDefStruct(type, name);
            }

            _writer.WriteLine();
        }

        return _writer.ToString();
    }

    private void GenerateTypeDefStruct(string type, string name)
    {
        _writer.WriteStructLayoutAttribute(LayoutKind.Sequential, _pack);
        using (_writer.WriteStruct(name, "public unsafe"))
        {
            // backing field
            if (SteamConverter.TryGetUnmanagedType(type, out var unmanagedType))
                _writer.WriteMarshalAsAttribute(unmanagedType);
            using (_writer.AppendContext())
            {
                _writer.Write("private ");
                _writer.Write(type);
                _writer.Write(" _value;");
            }

            _writer.WriteLine();

            // constructor
            using (_writer.AppendContext())
            {
                _writer.Write("public");
                _writer.Write(' ');
                _writer.Write(name);
                _writer.Write('(');
                _writer.Write(type);
                _writer.Write(" v) => _value = v;");
            }

            _writer.WriteLine();

            var isPointer = type.Contains('*');
            if (isPointer)
            {
                _writer.Write("public override int GetHashCode() => (int) _value;");
            }
            else
            {
                _writer.Write("public override bool Equals(object? obj) => obj is " + type +
                              " v && v.Equals(_value);");
                _writer.WriteLine();
                _writer.Write("public override int GetHashCode() => _value.GetHashCode();");
                _writer.WriteLine();
                _writer.Write("public override string ToString() => _value.ToString();");
            }

            _writer.WriteLine();

            // Backing->TypeDef operator
            using (_writer.AppendContext())
            {
                _writer.Write("public static implicit operator");
                _writer.Write(' ');
                _writer.Write(name);
                _writer.Write('(');
                _writer.Write(type);
                _writer.Write(" v) => new(v);");
            }

            _writer.WriteLine();


            // TypeDef->Backing operator
            using (_writer.AppendContext())
            {
                _writer.Write("public static implicit operator");
                _writer.Write(' ');
                _writer.Write(type);
                _writer.Write('(');
                _writer.Write(name);
                _writer.Write(" v) => v._value;");
            }

            _writer.WriteLine();
        }
    }

    public void GenerateTypeDefFixedSize(string type, string name, string fixedSize)
    {
        _writer.WriteStructLayoutAttribute(LayoutKind.Sequential, _pack);
        using (_writer.WriteStruct(name, "public unsafe"))
        {
            _writer.WriteConstant(new ConstantModel
            {
                Name = "Length",
                Type = "int",
                Value = fixedSize
            }, true);
            _writer.WriteLine();

            // backing field
            if (SteamConverter.TryGetUnmanagedType(type, out var unmanagedType))
                _writer.WriteMarshalAsAttribute(unmanagedType);
            using (_writer.AppendContext())
            {
                _writer.Write("private fixed ");
                _writer.Write(type);
                _writer.Write(" _value[");
                _writer.Write(fixedSize);
                _writer.Write("];");
            }

            _writer.WriteLine();


            // TypeDef->Backing operator
            using (_writer.AppendContext())
            {
                _writer.Write("public static implicit operator");
                _writer.Write(' ');
                _writer.Write(type);
                _writer.Write('*');
                _writer.Write('(');
                _writer.Write(name);
                _writer.Write(" v) => v._value;");
            }

            _writer.WriteLine();

            // Accessor
            using (_writer.AppendContext())
            {
                _writer.Write("public");
                _writer.Write(' ');
                _writer.Write(type);
                _writer.Write(' ');
                _writer.Write("this[int index]");
            }

            using (_writer.BlockContext())
            {
                _writer.Write("get => _value[index];");
                _writer.Write("set => _value[index] = value;");
            }
        }
    }

    public void GenerateTypeDefDelegate(string type, string name)
    {
        _writer.WriteUnmanagedFunctionPointerAttribute();
        using (_writer.AppendContext())
        {
            _writer.Write("public unsafe delegate void ");
            _writer.Write(name);
        }

        _writer.BeginBlock('(');

        var types = type.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (var i = 0; i < types.Length; i++)
        {
            var formatType = TypeConverter.ConvertType(types[i]);
            if (SteamConverter.TryGetUnmanagedType(formatType, out var parameterUnmanagedType))
                _writer.WriteMarshalAsAttribute(parameterUnmanagedType);

            using (_writer.AppendContext())
            {
                _writer.Write(formatType);
                _writer.Write(" arg" + i);
                if (i == types.Length - 1)
                    continue;

                _writer.Write(", ");
                _writer.WriteLine();
            }
        }

        _writer.EndBlock(')');
        _writer.Write(';');
    }
}