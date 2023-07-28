using System;
using System.Net.Http;

namespace Frontend.Services;

public interface IHttpClientFactory
{
    HttpClient Create();
    HttpClient Create(Uri uri);
    HttpClient Create(string uri);
}
