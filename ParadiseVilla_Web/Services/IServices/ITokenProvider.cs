using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(TokenDTO tokenDTO);
        TokenDTO? GetToken();
        void ClearToken();
    }
}
