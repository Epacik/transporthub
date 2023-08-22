using System;
using System.Net.Http;

namespace TransportHub.Services;

public interface IHttpClientFactory
{
    HttpClient Create();
    HttpClient Create(Uri uri, string? username = null, string? password = null);
    HttpClient Create(string uri, string? username = null, string? password = null);
}
