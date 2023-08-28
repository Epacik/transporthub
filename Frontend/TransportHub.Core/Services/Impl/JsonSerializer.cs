using Lindronics.OneOf.Result;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransportHub.Common;

namespace TransportHub.Core.Services.Impl;

internal class Serializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            new CustomDateTimeConverter("yyyy-MM-dd HH:mm:ss"),
        }
    };

    public Result<T?, Exception> Deserialize<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public Result<string?, Exception> Serialize<T>(T? value)
    {
        if (value is null)
            return new ArgumentNullException(nameof(value));

        try
        {
            return JsonSerializer.Serialize<T>(value, _options);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}


internal class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public CustomDateTimeConverter(string format, IFormatProvider? formatProvider = null)
    {
        Format = format;
        FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    }

    public string Format { get; }
    public IFormatProvider FormatProvider { get; }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString() ?? string.Empty, Format, FormatProvider);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, FormatProvider));
    }
}
