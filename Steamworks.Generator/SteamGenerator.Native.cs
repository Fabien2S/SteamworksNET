using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateNative()
    {
        Prepare();

        var hasOutput = false;
        using (_writer.WriteClass("SteamNative", "internal static unsafe partial"))
        {
            if (_model.NativeMethods != null)
            {
                foreach (var method in _model.NativeMethods)
                {
                    _writer.WriteMethodNative(_dllName, method, false);
                    _writer.WriteLine();
                    hasOutput = true;
                }
            }

            if (_model.Interfaces != null)
            {
                foreach (var @interface in _model.Interfaces)
                {
                    if (@interface.Accessors != null)
                    {
                        foreach (var accessor in @interface.Accessors)
                        {
                            _writer.WriteDllImportAttribute(_dllName, accessor.FlatName);
                            using (_writer.AppendContext())
                                _writer
                                    .Write("public static extern ")
                                    .Write(@interface.Name).Write(' ')
                                    .Write(accessor.FlatName)
                                    .Write("();");
                        }

                        _writer.WriteLine();
                    }

                    if (@interface.Methods != null)
                    {
                        foreach (var method in @interface.Methods)
                        {
                            _writer.WriteMethodNative(_dllName, method, true);
                            _writer.WriteLine();
                            hasOutput = true;
                        }
                    }
                }
            }

            if (_model.Structs != null)
            {
                foreach (var @struct in _model.Structs)
                {
                    if (@struct.Methods == null)
                        continue;

                    foreach (var method in @struct.Methods)
                    {
                        _writer.WriteMethodNative(_dllName, method, true);
                        _writer.WriteLine();
                        hasOutput = true;
                    }
                }
            }
        }

        return hasOutput ? _writer.ToString() : string.Empty;
    }
}