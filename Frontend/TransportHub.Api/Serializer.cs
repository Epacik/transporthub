using System.Text.Json;

namespace TransportHub.Api;

internal static class Serializer
{
    public static JsonSerializerOptions GetSerializerOptions()
        => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
}
