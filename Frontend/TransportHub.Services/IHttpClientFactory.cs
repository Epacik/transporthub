using System;
using System.Net.Http;

namespace TransportHub.Services;

public interface IHttpClientFactory
{
    HttpClient Create();
    HttpClient Create(Uri uri);
    HttpClient Create(string uri);
}
