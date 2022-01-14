using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateInterfaces()
    {
        if (_model.Interfaces == null)
            return string.Empty;

        Prepare();

        foreach (var interfaceModel in _model.Interfaces)
        {
            using (_writer.WriteClass(interfaceModel.Name, "internal static unsafe"))
            {
                if (interfaceModel.Accessors != null)
                {
                    foreach (var accessor in interfaceModel.Accessors)
                    {
                        _writer.Write("// " + accessor.Kind + ", " + accessor.Name + ", " + accessor.FlatName);
                        _writer.WriteDllImportAttribute(_dllName, accessor.FlatName);
                        using (_writer.AppendContext())
                            _writer.Write("public static extern IntPtr ").Write(accessor.FlatName).Write("();");
                    }

                    _writer.WriteLine();
                }

                if (interfaceModel.Fields != null)
                {
                    foreach (var field in interfaceModel.Fields)
                    {
                        _writer.WriteField(field, _model.TypeDefs);
                    }

                    _writer.WriteLine();
                }

                if (interfaceModel.Enums != null)
                {
                    foreach (var @enum in interfaceModel.Enums)
                    {
                        _writer.WriteEnum(@enum);
                        _writer.WriteLine();
                    }
                }

                if (interfaceModel.Methods != null)
                {
                    foreach (var method in interfaceModel.Methods)
                    {
                        _writer.WriteMethod(_dllName, method);
                        _writer.WriteLine();
                    }
                }
            }

            _writer.WriteLine();
        }

        return _writer.ToString();
    }
}