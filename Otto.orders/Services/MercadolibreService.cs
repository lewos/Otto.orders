using Microsoft.Extensions.Caching.Memory;
using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Models.Responses;
using System.Text.Json;

namespace Otto.orders.Services
{
    public class MercadolibreService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public MercadolibreService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _memoryCache = memoryCache;
            _cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(20)
                .SetSlidingExpiration(TimeSpan.FromSeconds(3000));
        }


        public async Task<MOrderResponse> GetMOrderCacheAsync(long MUserId, string Resource, string AccessToken)
        {
            var key = $"MOrderResponse_{Resource}";
            if (!_memoryCache.TryGetValue(key, out MOrderResponse response))
            {
                var mOrderResponse = await GetMOrderAsync(MUserId, Resource,AccessToken);
                _memoryCache.Set(key, mOrderResponse, _cacheEntryOptions);

                return mOrderResponse;
            }
            return response;
        }


        public async Task<MOrderResponse> GetMOrderAsync(long MUserId, string Resource, string AccessToken)
        {
            try
            {
                //TODO poner en una variable de entorno
                string baseUrl = "https://api.mercadolibre.com";
                string endpoint = Resource.Substring(1);
                string url = string.Join('/', baseUrl, endpoint);


                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, url);

                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var mOrder = await JsonSerializer.DeserializeAsync
                        <MOrderDTO>(contentStream);

                    //TODO comentar
                    string jsonString = JsonSerializer.Serialize(mOrder);
                    Console.WriteLine(jsonString);


                    return new MOrderResponse(Response.OK, $"{Response.OK}", mOrder);

                }
                // TODO verificar si es 401 o sin permiso ver lo del refresh



                //TODO si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MOrderResponse(Response.WARNING, $"No existe la orden {Resource} del usuario {MUserId}", null);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}");
                //TODO verificar en donde leo la respuesta del servicio
                return new MOrderResponse(Response.ERROR, $"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}", null);

            }
        }

        public async Task<MItemResponse> GetMItemAsync(long MUserId, string Resource, string AccessToken)
        {
            try
            {
                //TODO poner en una variable de entorno
                string baseUrl = "https://api.mercadolibre.com";
                string endpoint = Resource;
                string url = string.Join('/', baseUrl, endpoint);


                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, url);

                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var mItem = await JsonSerializer.DeserializeAsync
                        <MItemDTO>(contentStream);


                    return new MItemResponse(Response.OK, $"{Response.OK}", mItem);

                }

                //TODO si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MItemResponse(Response.WARNING, $"No existe la orden {Resource} del usuario {MUserId}", null);


            }
            catch (Exception ex)
            {
                //TODO verificar en donde leo la respuesta del servicio
                return new MItemResponse(Response.ERROR, $"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}", null);

            }


        }
    }
}
