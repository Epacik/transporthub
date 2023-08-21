using TransportHub.Common;
using System;
using System.Net.Http;
using TransportHub.Services;

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
        var uri = _settingsService.Read(Settings.IpAddress, "http://127.0.0.1:8080");
        return Create(uri);
    }

    public HttpClient Create(Uri uri)
    {
        return new HttpClient
        {
            BaseAddress = uri,
        };
    }

    public HttpClient Create(string uri) => Create(new Uri(uri));
}
