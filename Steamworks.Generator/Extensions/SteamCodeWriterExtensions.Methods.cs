using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

namespace Steamworks.Generator.Extensions;

public static partial class SteamCodeWriterExtensions
{
    public static void WriteMethodNative(this CodeWriter writer, MethodModel method, string? selfParameter)
    {
        TypeFormatter.FormatMethodNative(ref method);

        writer.WriteDllImportAttribute(method.FlatName);

        if (SteamConverter.TryGetUnmanagedType(method.ReturnType, out var unmanagedType))
        {
            var unmanagedTypeName = Enum.GetName(unmanagedType);
            writer.Write($"[return: MarshalAs(UnmanagedType.{unmanagedTypeName})]");
        }

        writer.BeginBlock($"public static extern {method.ReturnType} {method.FlatName}(");
        {
            var parameters = method.Parameters;
            if (parameters is {Length: > 0})
            {
                if (selfParameter != null)
                    writer.Write($"{selfParameter} self, ");

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    using (writer.AppendContext())
                    {
                        if (SteamConverter.TryGetUnmanagedType(parameter.Type, out var parameterUnmanagedType))
                        {
                            writer.WriteMarshalAsAttribute(parameterUnmanagedType);
                            writer.Write(' ');
                        }

                        writer.Write($"{parameter.Type} {parameter.Name}");
                        if (i != parameters.Length - 1)
                            writer.Write(", ");
                    }
                }
            }
            else if (selfParameter != null)
                writer.Write($"{selfParameter} self");
        }
        writer.EndBlock(");");
    }

    public static void WriteMethodFacing(this CodeWriter writer, MethodModel method, string? selfParameter)
    {
        if (!TypePredicate.ShouldIncludeMethod(in method))
            return;

        TypeFormatter.FormatMethodFacing(ref method);

        writer.Write("/// <summary>");
        writer.Write($"/// {method.FlatName}");
        writer.Write("/// </summary>");

        if (method.Parameters != null)
        {
            foreach (var parameter in method.Parameters)
                writer.Write($"/// <param name=\"{parameter.Name}\">{parameter.Description}</param>");
        }

        if (!string.IsNullOrEmpty(method.CallResult))
            writer.Write($"/// <seealso cref=\"{method.CallResult}\"/>");

        if (!string.IsNullOrEmpty(method.Callback))
            writer.Write($"/// <seealso cref=\"{method.Callback}\"/>");

        using (writer.AppendContext())
        {
            writer.Write("public ");
            if (selfParameter == null) writer.Write("static ");
            writer.Write($"{method.ReturnType} {method.Name}(");

            // Parameters
            var parameters = method.Parameters;
            if (parameters is {Length: > 0})
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    writer
                        .Write(parameter.Type)
                        .Write(' ')
                        .Write(parameter.Name);

                    if (i != parameters.Length - 1)
                        writer.Write(", ");
                }
            }

            writer.Write(") => ");

            // SteamNative
            writer.Write("SteamNative.").Write(method.FlatName).Write('(');

            // Self parameter
            if (selfParameter != null)
                writer.Write(selfParameter);

            // Parameters
            if (parameters is {Length: > 0})
            {
                if (selfParameter != null) writer.Write(", ");
                for (var i = 0; i < parameters.Length; i++)
                {
                    writer.Write(parameters[i].Name);
                    if (i != parameters.Length - 1)
                        writer.Write(", ");
                }
            }

            writer.Write(");");
        }
    }
}