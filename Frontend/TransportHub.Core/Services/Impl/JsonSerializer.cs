using Lindronics.OneOf.Result;
using System;
using System.Text.Json;
using TransportHub.Common;

namespace TransportHub.Core.Services.Impl;

internal class Serializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
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
