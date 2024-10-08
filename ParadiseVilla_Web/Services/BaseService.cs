﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using ParadiseVilla_Utility;
using ParadiseVilla_Web.Models;
using ParadiseVilla_Web.Models.DTO;
using ParadiseVilla_Web.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace ParadiseVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse reponseModel { get; set; }
        public IHttpClientFactory httpClient {  get; set; } 
        private readonly ITokenProvider _tokenProvider; 
        protected readonly string VillaApiUrl;
        private IHttpContextAccessor _httpContextAccessor;
        public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            reponseModel = new APIResponse();
            this.httpClient = httpClient;
            this._tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
            VillaApiUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest, bool withBearer)
        {
            try
            {
                var client = httpClient.CreateClient("VillaAPI");
                var messageFactory = () =>
                {
                    HttpRequestMessage message = new HttpRequestMessage();
                    if (apiRequest.ContentType == SD.ContentType.MultipartFormData)
                    {
                        message.Headers.Add("Accept", "*/*");
                    }
                    else
                    {
                        message.Headers.Add("Accept", "application/json");
                    }
                    message.RequestUri = new Uri(apiRequest.Url);

                    if (apiRequest.ContentType == SD.ContentType.MultipartFormData)
                    {
                        //
                        var content = new MultipartFormDataContent();
                        foreach (var prop in apiRequest.Data.GetType().GetProperties())
                        {
                            var value = prop.GetValue(apiRequest.Data);
                            if (value is FormFile)
                            {
                                var file = (FormFile)value;
                                if (file != null)
                                {
                                    content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                                }
                            }
                            else
                            {
                                content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                            }
                            message.Content = content;
                        }
                    }
                    else
                    {
                        if (apiRequest.Data != null)
                        {
                            message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                        }
                    }

                    switch (apiRequest.ApiType)
                    {
                        case SD.ApiType.POST:
                            message.Method = HttpMethod.Post;
                            break;
                        case SD.ApiType.PUT:
                            message.Method = HttpMethod.Put;
                            break;
                        case SD.ApiType.DELETE:
                            message.Method = HttpMethod.Delete;
                            break;
                        default:
                            message.Method = HttpMethod.Get;
                            break;
                    }
                    return message;
                };

                HttpResponseMessage httpResponseMessage = null;
                httpResponseMessage = await SendWithRefreshTokenAsync(client, messageFactory, withBearer);
                APIResponse FinalApiResponse = new()
                {
                    IsSuccess = false
                };
                try
                {
                    switch (httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            FinalApiResponse.Errors = new List<string>() { "Not Found" };
                            break;
                        case HttpStatusCode.Forbidden:
                            FinalApiResponse.Errors = new List<string>() { "Access Denied" };
                            break;
                        case HttpStatusCode.Unauthorized:
                            FinalApiResponse.Errors = new List<string>() { "Unauthorized" };
                            break;
                        case HttpStatusCode.InternalServerError:
                            FinalApiResponse.Errors = new List<string>() { "Internal Server Error" };
                            break;
                        default:
                            var apiContent = await httpResponseMessage.Content.ReadAsStringAsync();
                            FinalApiResponse.IsSuccess = true;
                            FinalApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                            break;
                    }

                }
                catch (Exception ex)
                {
                    FinalApiResponse.Errors = new List<string>() { "Error Encountered", ex.Message.ToString() };
                }
                var res = JsonConvert.SerializeObject(FinalApiResponse);
                var returnObj = JsonConvert.DeserializeObject<T>(res);
                return returnObj;

            }
            catch (AuthException)
            {
                throw;
            }
            catch (Exception e)
            {
                var dto = new APIResponse
                {
                    Errors = new List<string> { Convert.ToString(e.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
        private async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient httpClient,
            Func<HttpRequestMessage> httpRequestMessageFactory, bool withBearer = true)
        {
            if (!withBearer)
            {
                return await httpClient.SendAsync(httpRequestMessageFactory());
            }
            else
            {
                TokenDTO tokenDTO = _tokenProvider.GetToken();
                if (tokenDTO != null && !string.IsNullOrEmpty(tokenDTO.AccessToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDTO.AccessToken);
                }

                try
                {
                    var response = await httpClient.SendAsync(httpRequestMessageFactory());
                    if (response.IsSuccessStatusCode)
                        return response;

                    // IF this fails then we can pass refresh token!
                    if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //GENERATE NEW Token from Refresh token / Sign in with that new token and then retry
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        response = await httpClient.SendAsync(httpRequestMessageFactory());
                        return response;
                    }

                    return response;

                }
                catch (AuthException)
                {
                    throw;
                }
                catch (HttpRequestException httpRequestException)
                {
                    if (httpRequestException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // refresh token and retry the request
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        return await httpClient.SendAsync(httpRequestMessageFactory());
                    }
                    throw;
                }
            }
        }

        private async Task InvokeRefreshTokenEndpoint(HttpClient httpClient, string existingAccessToken, string existingRefreshToken)
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri($"{VillaApiUrl}/api/{SD.CurrentAPIVersion}/UserAuth/refresh");
            message.Method = HttpMethod.Post;
            message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDTO()
            {
                AccessToken = existingAccessToken,
                RefreshToken = existingRefreshToken
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);

            if (apiResponse?.IsSuccess != true)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw new AuthException();
            }
            else
            {
                var tokenDataStr = JsonConvert.SerializeObject(apiResponse.Result);
                var tokenDto = JsonConvert.DeserializeObject<TokenDTO>(tokenDataStr);

                if (tokenDto != null && !string.IsNullOrEmpty(tokenDto.AccessToken))
                {
                    //New method to sign in with the new token that we receive
                    await SignInWithNewTokens(tokenDto);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
                }
            }
        }

        private async Task SignInWithNewTokens(TokenDTO tokenDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
            var principal = new ClaimsPrincipal(identity);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _tokenProvider.SetToken(tokenDTO);
        }


    }
}
