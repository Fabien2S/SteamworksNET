using System.Runtime.InteropServices;
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

        using (CodeWriterContext())
        {
            foreach (var typeDefinition in _model.TypeDefs)
            {
                var name = typeDefinition.Name;

                var formattedName = TypeConverter.ConvertType(typeDefinition.Name);
                if (!string.Equals(name, formattedName, StringComparison.Ordinal))
                    continue;

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
        }

        return _writer.ToString();
    }

    private void GenerateTypeDefStruct(string type, string name)
    {
        var isPointer = type.EndsWith('*');
        var backType = isPointer ? "IntPtr" : type;

        _writer.WriteStructLayoutAttribute(LayoutKind.Sequential);
        using (_writer.WriteBlock($"public readonly unsafe struct {name} : IEquatable<{name}>, IEquatable<{backType}>"))
        {
            // backing field
            if (SteamConverter.TryGetUnmanagedType(type, out var unmanagedType))
                _writer.WriteMarshalAsAttribute(unmanagedType);

            _writer.Write($"private readonly {backType} _value;");
            _writer.WriteLine();

            // constructor
            _writer.Write(isPointer
                ? $"public {name}({type} v) => _value = (IntPtr) v;"
                : $"public {name}({type} v) => _value = v;");
            _writer.WriteLine();

            _writer.Write($"public bool Equals({name} other) => _value == other._value;");
            _writer.WriteLine();

            _writer.Write($"public bool Equals({backType} other) => _value == other;");
            _writer.WriteLine();

            _writer.Write($"public override bool Equals(object obj) => obj is {name} v && Equals(v);");
            _writer.WriteLine();

            _writer.Write("public override int GetHashCode() => _value.GetHashCode();");
            _writer.WriteLine();

            _writer.Write(isPointer
                ? "public override string ToString() => _value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);"
                : "public override string ToString() => _value.ToString();");
            _writer.WriteLine();

            // Backing->TypeDef operator
            _writer.Write($"public static implicit operator {name}({type} v) => new(v);");
            _writer.WriteLine();

            // TypeDef->Backing operator
            if (isPointer)
            {
                _writer.Write($"public static implicit operator IntPtr({name} v) => v._value;");
                _writer.WriteLine();

                _writer.Write($"public static implicit operator {type}({name} v) => (void*) v._value;");
            }
            else
                _writer.Write($"public static implicit operator {type}({name} v) => v._value;");
            _writer.WriteLine();
        }
    }

    private void GenerateTypeDefFixedSize(string type, string name, string fixedSize)
    {
        _writer.WriteStructLayoutAttribute(LayoutKind.Sequential);
        using (_writer.WriteBlock($"public unsafe struct {name}"))
        {
            _writer.WriteConstant(new ConstantModel
            {
                Name = "Length",
                Type = "int",
                Value = fixedSize
            });
            _writer.WriteLine();

            // backing field
            if (SteamConverter.TryGetUnmanagedType(type, out var unmanagedType))
                _writer.WriteMarshalAsAttribute(unmanagedType);

            _writer.Write($"private fixed {type} _value[{fixedSize}];");
            _writer.WriteLine();


            // TypeDef->Backing operator
            _writer.Write($"public static implicit operator {type}*({name} v) => v._value;");
            _writer.WriteLine();

            // Accessor
            using (_writer.WriteBlock($"public {type} this[int index]"))
            {
                _writer.Write("get => _value[index];");
                _writer.Write("set => _value[index] = value;");
            }
        }
    }

    private void GenerateTypeDefDelegate(string type, string name)
    {
        _writer.WriteUnmanagedFunctionPointerAttribute();
        _writer.BeginBlock($"public unsafe delegate void {name}(");
        {
            var types = type.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (var i = 0; i < types.Length; i++)
            {
                var formatType = TypeConverter.ConvertType(types[i]);
                using (_writer.AppendContext())
                {
                    if (SteamConverter.TryGetUnmanagedType(formatType, out var parameterUnmanagedType))
                    {
                        _writer.WriteMarshalAsAttribute(parameterUnmanagedType);
                        _writer.Write(' ');
                    }

                    _writer.Write($"{formatType} arg{i}");
                    if (i != types.Length - 1)
                        _writer.Write(',');
                }
            }
        }
        _writer.EndBlock(");");
    }
}