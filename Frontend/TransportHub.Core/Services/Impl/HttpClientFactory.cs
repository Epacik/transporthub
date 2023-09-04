using TransportHub.Common;
using System;
using System.Net.Http;
using TransportHub.Services;
using System.Net.Http.Headers;
using Discord.Net;

namespace TransportHub.Core.Services.Impl;

internal class HttpClientFactory : IHttpClientFactory
{
    private readonly ISettingsService _settingsService;

    public HttpClientFactory(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public HttpClient Create()
    {
        var uri = _settingsService.Read(Settings.IpAddress, DefaultValues.ServerAddress);
        return Create(uri);
    }

    public HttpClient Create(Uri uri, string? username = null, string? password = null)
    {
        var client = new HttpClient
        {
            BaseAddress = uri,
        };

        if (username is not null || password is not null)
        {
            var val = $"{username}:{password}";
            var valueBytes = System.Text.Encoding.UTF8.GetBytes(val);
            var base64 = System.Convert.ToBase64String(valueBytes);
            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", base64);
        }
        //client.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
        //client.DefaultRequestHeaders.Add("mode", "no-cors");

        return client;
    }

    public HttpClient Create(string uri, string? username = null, string? password = null)
        => Create(new Uri(uri), username, password);
}
