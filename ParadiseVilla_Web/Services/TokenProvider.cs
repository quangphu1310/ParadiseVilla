using ParadiseVilla_Utility;
using ParadiseVilla_Web.Models.DTO;
using ParadiseVilla_Web.Services.IServices;

namespace ParadiseVilla_Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor; 
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
        }

        public TokenDTO GetToken()
        {
            try
            {   
                bool hasAccessToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string accessToken);
                TokenDTO tokenDTO = new TokenDTO()
                {
                    AccessToken = accessToken,
                };
                return hasAccessToken ? tokenDTO : null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public void SetToken(TokenDTO tokenDTO)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken, tokenDTO.AccessToken, cookieOptions);
        }
    }
}
