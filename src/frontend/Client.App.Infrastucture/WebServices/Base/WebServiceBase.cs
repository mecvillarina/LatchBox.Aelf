using MimeMapping;
using Newtonsoft.Json;
using Application.Common.Models;
using Client.Infrastructure.Enums;
using Client.Infrastructure.Exceptions;
using Client.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices.Base
{
    public class WebServiceBase
    {
        private readonly AppHttpClient _appHttpClient;

        public WebServiceBase(AppHttpClient appHttpClient)
        {
            _appHttpClient = appHttpClient;
        }

        protected async Task<byte[]> GetStreamAsync(string requestUri, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.GET, requestUri, accessToken))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);

                        return ms.ToArray();
                    }
                }
            }
        }

        protected async Task<IResult> GetAsync(string requestUri, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.GET, requestUri, accessToken))
            {
                return await response.ToResult();
            }
        }

        protected async Task<IResult<TResult>> GetAsync<TResult>(string requestUri, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.GET, requestUri, accessToken))
            {
                return await response.ToResult<TResult>();
            }
        }

        protected async Task<IResult> PostAsync<TValue>(string requestUri, TValue contract, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.POST, requestUri, contract, accessToken))
            {
                return await response.ToResult();
            }
        }

        protected async Task<IResult<TResult>> PostAsync<TValue, TResult>(string requestUri, TValue contract, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.POST, requestUri, contract, accessToken))
            {
                return await response.ToResult<TResult>();
            }
        }

        protected async Task<IResult> PostFileAsync<TValue>(string requestUri, TValue contract, Stream fileStream, string filename, string accessToken = null)
        {
            using (var response = await InternalMultipartFormDataRequestAsync(HttpVerb.POST, requestUri, contract, fileStream, filename, accessToken))
            {
                return await response.ToResult();
            }
        }

        protected async Task<IResult<TResult>> PostFileAsync<TValue, TResult>(string requestUri, TValue contract, Stream fileStream, string filename, string accessToken = null)
        {
            using (var response = await InternalMultipartFormDataRequestAsync(HttpVerb.POST, requestUri, contract, fileStream, filename, accessToken))
            {
                return await response.ToResult<TResult>();
            }
        }

        protected async Task<IResult> PutAsync<TValue>(string requestUri, TValue contract, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.PUT, requestUri, contract, accessToken))
            {
                return await response.ToResult();
            }
        }

        protected async Task<IResult<TResult>> PutAsync<TValue, TResult>(string requestUri, TValue contract, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.PUT, requestUri, contract, accessToken))
            {
                return await response.ToResult<TResult>();
            }

        }
        protected async Task<IResult> PatchAsync<TValue>(string requestUri, TValue contract, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.PATCH, requestUri, contract, accessToken))
            {
                return await response.ToResult();
            }
        }

        protected async Task<IResult<TResult>> PatchAsync<TValue, TResult>(string requestUri, TValue contract, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.PATCH, requestUri, contract, accessToken))
            {
                return await response.ToResult<TResult>();
            }
        }

        protected async Task<IResult> DeleteAsync(string requestUri, string accessToken = null)
        {
            using (var response = await InternalJsonRequestAsync(HttpVerb.DELETE, requestUri, accessToken))
            {
                return await response.ToResult();
            }
        }

        private async Task<HttpResponseMessage> InternalMultipartFormDataRequestAsync<TValue>(HttpVerb httpRequestType, string requestUri, TValue contract, Stream fileStream, string filename, string accessToken)
        {
            using (var formHttpContent = new MultipartFormDataContent())
            {
                if (contract != null)
                {
                    formHttpContent.Add(new StringContent(JsonConvert.SerializeObject(contract), Encoding.UTF8, KnownMimeTypes.Json), "JsonPayload");
                }

                formHttpContent.Add(new StreamContent(fileStream), "AttachedFile", filename);

                return await InternalRequestAsync(httpRequestType, requestUri, formHttpContent, accessToken);
            }
        }

        private async Task<HttpResponseMessage> InternalJsonRequestAsync<TValue>(HttpVerb httpRequestType, string requestUri, TValue contract, string accessToken)
        {
            if (contract != null)
            {
                using (var jsonHttpContent = EncodeObjectToJsonHttpContent(contract))
                {
                    return await InternalRequestAsync(httpRequestType, requestUri, jsonHttpContent, accessToken);
                }
            }
            else
            {
                return await InternalJsonRequestAsync(httpRequestType, requestUri, accessToken);
            }
        }

        private HttpContent EncodeObjectToJsonHttpContent<T>(T content)
        {
            return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, KnownMimeTypes.Json);
        }

        private async Task<HttpResponseMessage> InternalJsonRequestAsync(HttpVerb httpRequestType, string requestUri, string accessToken)
        {
            return await InternalRequestAsync(httpRequestType, requestUri, null, accessToken);
        }

        protected async Task<HttpResponseMessage> InternalRequestAsync(HttpVerb httpRequestType, string requestUri, HttpContent httpContent, string accessToken)
        {
            if (_appHttpClient.DefaultRequestHeaders != null)
            {
                _appHttpClient.DefaultRequestHeaders.Clear();
                _appHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(KnownMimeTypes.Json));

                if (!string.IsNullOrEmpty(accessToken))
                {
                    _appHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }

            var response = await _appHttpClient.RequestAsync(httpRequestType, requestUri, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;

                if (statusCode == 401)
                {
                    throw new SessionExpiredException();
                }
            }

            return response;
        }
    }
}
