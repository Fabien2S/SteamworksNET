using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

namespace Steamworks.Generator.Extensions;

public static partial class SteamCodeWriterExtensions
{
    public static void WriteConstant(this CodeWriter writer, ConstantModel constant)
    {
        TypeFormatter.FormatConstant(ref constant);

        writer.Write(SteamConverter.TryGetUnmanagedType(constant.Type, out _)
            ? $"public const {constant.Type} {constant.Name} = {constant.Value};"
            : $"public static readonly {constant.Type} {constant.Name} = {constant.Value};");
    }

    public static void WriteEnum(this CodeWriter writer, EnumModel @enum)
    {
        TypeFormatter.FormatEnum(ref @enum);

        using (writer.WriteBlock($"public enum {@enum.Name}"))
        {
            foreach (var enumValue in @enum.Values)
                writer.Write($"{enumValue.Name} = {enumValue.Value},");
        }
    }

    public static void WriteField(this CodeWriter writer, FieldModel field, TypeDefModel[]? typeDefs)
    {
        if (!TypePredicate.ShouldIncludeField(in field))
            return;

        TypeFormatter.FormatField(ref field);

        if (field.CustomAttribute != null)
            writer.Write(field.CustomAttribute);

        using (writer.AppendContext())
        {
            writer.Write(field.IsPrivate ? "private" : "public").Write(' ');

            if (SteamConverter.IsFixedSizeArrayType(field.Type, out var fixedType, out var fixedSize))
            {
                writer.Write("fixed").Write(' ');

                var formatType = SteamFormatter.FormatFixedSizeType(fixedType, typeDefs);
                writer.Write(formatType).Write(' ');
                writer.Write(field.Name).Write('[').Write(fixedSize).Write(']');
                writer.Write(';');
            }
            else
            {
                writer.Write(field.Type).Write(' ');
                writer.Write(field.Name);
                writer.Write(';');
            }
        }
    }
}