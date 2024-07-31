using ParadiseVilla_API.Models;
using ParadiseVilla_API.Models.DTO;

namespace ParadiseVilla_API.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser (string username);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
