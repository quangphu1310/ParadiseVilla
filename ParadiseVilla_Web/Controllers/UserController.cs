using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ParadiseVilla_Web.Models;
using ParadiseVilla_Web.Models.DTO;
using ParadiseVilla_Web.Services.IServices;

namespace ParadiseVilla_Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IAuthService _authService;
        private IMapper _mapper;
        public UserController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        public ActionResult Login() {

            LoginRequestDTO obj = new();
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            return RedirectToAction("Index","Villa");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            var response = await _authService.RegisterAsync<APIResponse>(obj);
            if(response != null && response.IsSuccess) 
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
