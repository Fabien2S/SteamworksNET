using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateNative()
    {
        var hasOutput = false;

        using (CodeWriterContext())
        {
            using (_writer.WriteBlock("internal static unsafe partial class SteamNative"))
            {
                if (_model.NativeMethods != null)
                {
                    foreach (var method in _model.NativeMethods)
                    {
                        _writer.WriteMethodNative(method, null);
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
                                _writer.WriteDllImportAttribute(accessor.FlatName);
                                _writer.Write($"public static extern {@interface.Name} {accessor.FlatName}();");
                            }

                            _writer.WriteLine();
                        }

                        if (@interface.Methods != null)
                        {
                            foreach (var method in @interface.Methods)
                            {
                                _writer.WriteMethodNative(method, "IntPtr");
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
                            _writer.WriteMethodNative(method, "ref " + @struct.Name);
                            _writer.WriteLine();
                            hasOutput = true;
                        }
                    }
                }
            }
        }

        return hasOutput ? _writer.ToString() : string.Empty;
    }
}