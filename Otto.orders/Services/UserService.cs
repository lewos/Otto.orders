﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Models.Responses;
using System.Text.Json;

namespace Otto.orders.Services
{
    public class UserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public UserService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(20)
                .SetSlidingExpiration(TimeSpan.FromSeconds(3000));
        }


        public async Task<UserResponse> GetUserByMIdCacheAsync(long MUserId)
        {
            var key = $"UserByMId_{MUserId}";
            if (!_memoryCache.TryGetValue(key, out UserResponse response))
            {
                var userResponse = await GetUserByMIdAsync(MUserId);
                _memoryCache.Set(key, userResponse, _cacheEntryOptions);

                return userResponse;
            }
            return response;
        }

        public async Task<UserResponse> GetUserByMIdAsync(long MUserId)
        {
            try
            {
                //Deberia estar dentro de una variable de entorno
                string baseUrl = "https://ottousers.herokuapp.com";
                string endpoint = "api/Users/GetByMUserId";
                string url = string.Join('/', baseUrl, endpoint, MUserId);


                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, url)
                {
                    Headers =
                    {
                        { HeaderNames.Accept, "*/*" },
                    }
                };

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var userDTO = await JsonSerializer.DeserializeAsync
                        <UserDTO>(contentStream);                    

                    return new UserResponse(Response.OK, $"{Response.OK}", userDTO);

                }

                //si no lo encontro, verificar en donde leo la respuesta del servicio
                return new UserResponse(Response.WARNING, $"No existe el usuario con el id {MUserId}", null);


            }
            catch (Exception ex)
            {
                //verificar en donde leo la respuesta del servicio
                return new UserResponse(Response.ERROR, $"Error al obtener el usuario con id {MUserId}. Ex : {ex}", null);

            }


        }
    }
}
