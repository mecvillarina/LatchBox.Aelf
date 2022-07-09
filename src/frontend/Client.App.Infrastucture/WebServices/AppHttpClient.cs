using Client.Infrastructure.Enums;
using Client.Infrastructure.Exceptions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices
{
    public sealed class AppHttpClient : IDisposable
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(2);
        private static readonly object Locker = new object();

        private readonly HttpClient _httpClient;

        public HttpRequestHeaders DefaultRequestHeaders => _httpClient.DefaultRequestHeaders;

        public Uri BaseAddress
        {
            get => _httpClient.BaseAddress;
            set => _httpClient.BaseAddress = value;
        }

        public AppHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> RequestAsync(HttpVerb httpVerb, string requestUri,
            HttpContent content = null)
        {
            var method = httpVerb.ToString().ToUpper();
            var absoluteRequestUri = $"{BaseAddress}{requestUri}";

            try
            {
                //_debugLogger.Info($"{method} Sending {absoluteRequestUri}");

                HttpResponseMessage response;

                switch (httpVerb)
                {
                    case HttpVerb.DELETE:
                        response = await _httpClient.DeleteAsync(requestUri);
                        break;
                    case HttpVerb.POST:
                        response = await _httpClient.PostAsync(requestUri, content);
                        break;
                    case HttpVerb.PUT:
                        response = await _httpClient.PutAsync(requestUri, content);
                        break;
                    case HttpVerb.GET:
                        response = await _httpClient.GetAsync(requestUri);
                        break;
                    default:
                        response = await _httpClient.SendAsync(
                            new HttpRequestMessage(new HttpMethod(method), requestUri) { Content = content });
                        break;
                }

                var statusCode = (int)response.StatusCode;
                return response;
            }
            catch (OperationCanceledException oex) when (oex.CancellationToken.IsCancellationRequested)
            {
                throw new GeneralException($"Sorry, we're having trouble with our server.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Failed to fetch"))
                {
                    throw new GeneralException($"Cannot fetch data. Please check your internet connection.");
                }

                throw new GeneralException($"Sorry, we're having trouble with our server.");
            }
        }

        public void CancelAllRequestPending()
        {
            _httpClient?.CancelPendingRequests();
        }

        public void AddHeader(string key, string value)
        {
            lock (Locker)
            {
                if (_httpClient.DefaultRequestHeaders.Contains(key))
                {
                    _httpClient.DefaultRequestHeaders.Remove(key);
                }

                _httpClient.DefaultRequestHeaders.Add(key, value);
            }
        }

        public void RemoveHeader(string key)
        {
            lock (Locker)
            {
                if (_httpClient.DefaultRequestHeaders.Contains(key))
                {
                    _httpClient.DefaultRequestHeaders.Remove(key);
                }
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
