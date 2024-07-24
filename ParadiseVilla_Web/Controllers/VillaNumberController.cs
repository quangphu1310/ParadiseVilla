using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ParadiseVilla_Utility;
using ParadiseVilla_Web.Models;
using ParadiseVilla_Web.Models.DTO;
using ParadiseVilla_Web.Models.ViewModel;
using ParadiseVilla_Web.Services.IServices;

namespace ParadiseVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        public VillaNumberController(IMapper mapper, IVillaNumberService villaNumberService, IVillaService villaService)
        {
            _mapper = mapper;
            _villaNumberService = villaNumberService;
            _villaService = villaService;
        }
        public async Task<IActionResult> Index()
        {
            List<VillaNumberDTO> list = null;
            var response = await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create() {
            VillaNumberCreateVM villaNumberVM = new();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(villaNumberVM);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(VillaNumberCreateVM villaNumber)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(villaNumber.VillaNumberCreateDTO, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = $"Create {villaNumber.VillaNumberCreateDTO.VillaNo} successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (response.Errors.Count > 0)
                    {
                        ModelState.AddModelError("ErrorsMessage", response.Errors.FirstOrDefault());
                    }
                }
            }
            var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (resp != null && resp.IsSuccess)
            {
                villaNumber.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(villaNumber);
        }
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Update(int id)
        {
            VillaNumberUpdateVM villaNumberVM = new();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            var responseVillaNumber = await _villaNumberService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess && responseVillaNumber != null)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                var villaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(responseVillaNumber.Result));
                villaNumberVM.VillaNumberUpdateDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
            }
            return View(villaNumberVM);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Update(VillaNumberUpdateVM villaNumber)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(villaNumber.VillaNumberUpdateDTO, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = $"Update {villaNumber.VillaNumberUpdateDTO.VillaNo} successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Delete(int id)
        {
            VillaNumberDeleteVM villaNumberVM = new();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            var responseVillaNumber = await _villaNumberService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess && responseVillaNumber != null)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                var villaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(responseVillaNumber.Result));
                villaNumberVM.VillaNumberDTO = _mapper.Map<VillaNumberDTO>(villaNumber);
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Delete(VillaNumberDeleteVM villaNumber)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.DeleteAsync<APIResponse>(villaNumber.VillaNumberDTO.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = $"Delete {villaNumber.VillaNumberDTO.VillaNo} successfully!";
                    return RedirectToAction(nameof(Index));

                }
            }
            return View();
        }
    }
}
