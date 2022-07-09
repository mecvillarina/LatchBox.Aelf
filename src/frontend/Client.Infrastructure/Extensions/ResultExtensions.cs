using Application.Common.Exceptions;
using Application.Common.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Client.Infrastructure.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;

                switch (statusCode)
                {
                    case 403:
                    case 404:
                        throw new ForbiddenAccessException();
                }
            }

            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }

        public static async Task<IResult> ToResult(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;

                switch (statusCode)
                {
                    case 403:
                    case 404:
                        throw new ForbiddenAccessException();
                }
            }

            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject;
        }
    }
}
