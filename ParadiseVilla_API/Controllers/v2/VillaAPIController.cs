﻿using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ParadiseVilla_API.Data;
using ParadiseVilla_API.Models;
using ParadiseVilla_API.Models.DTO;
using ParadiseVilla_API.Repository.IRepository;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ParadiseVilla_API.Controllers.v2
{
    [Route("/api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaAPIController : ControllerBase
    {
        private readonly IVillaRepository _dbVilla;
        private readonly APIResponse _response;
        private IMapper _mapper;
        public VillaAPIController(IVillaRepository db, IMapper mapper)
        {
            _dbVilla = db;
            _mapper = mapper;
            _response = new APIResponse();
        }
        //[ResponseCache(CacheProfileName = "Default30")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "FilterOccupancy")] int? occupancy
            , [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villaList;
                if (occupancy > 0)
                {
                    villaList = await _dbVilla.GetAllAsync(x => x.Occupancy == occupancy);
                }
                else
                {
                    villaList = await _dbVilla.GetAllAsync();
                }
                if (!string.IsNullOrEmpty(search))
                {
                    villaList = await _dbVilla.GetAllAsync(x => x.Name.ToLower().Contains(search.ToLower()));
                }

                villaList = await _dbVilla.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                Pagination pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("pagination", JsonConvert.SerializeObject(pagination));
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villaStore = await _dbVilla.GetAsync(x => x.Id == id);
                if (villaStore == null)
                {
                    _response.IsSuccess = false;
                    return NotFound();
                }
                else
                    _response.Result = _mapper.Map<VillaDTO>(villaStore);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromForm] VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("Errors", "The Villa Already Exists!");
                    return BadRequest(ModelState);
                }
                Villa villa = _mapper.Map<Villa>(villaDTO);
                await _dbVilla.CreateAsync(villa);

                //Xử lí ảnh
                if(villaDTO.Image != null)
                {
                    // Tạo tên file bằng cách lấy Id của villa và phần mở rộng của file ảnh
                    string fileName = villa.Id + Path.GetExtension(villaDTO.Image.FileName);
                    // Xác định đường dẫn lưu file trong thư mục wwwroot\ProductImage
                    string filePath = @"wwwroot\VillaImage\" + fileName;

                    // Lấy vị trí đầy đủ của file trên hệ thống
                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    // Tạo đối tượng FileInfo để làm việc với file
                    FileInfo file = new FileInfo(directoryLocation);

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    // Sử dụng FileStream để tạo file mới và sao chép nội dung ảnh vào
                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        villaDTO.Image.CopyTo(fileStream);
                    }

                    // Tạo URL để truy cập ảnh từ trình duyệt
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    villa.ImageUrl = baseUrl + "/VillaImage/" + fileName; // Lưu URL của ảnh vào thuộc tính ImageUrl của villa
                    villa.ImageLocalPath = filePath; // Lưu đường dẫn local của ảnh vào thuộc tính ImageLocalPath của villa
                }
                else
                {
                    villa.ImageUrl = "https://placehold.co/600x400";
                }
                await _dbVilla.UpdateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villaToDelete = await _dbVilla.GetAsync(x => x.Id == id);
                if (villaToDelete == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                if (!string.IsNullOrEmpty(villaToDelete.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), villaToDelete.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                await _dbVilla.RemoveAsync(villaToDelete);
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [Authorize(Roles = "admin", AuthenticationSchemes = "Bearer")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromForm] VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null || id != villaDTO.Id)
                {
                    return BadRequest();
                }
                var villaToUpdate = _mapper.Map<Villa>(villaDTO);

                if (villaDTO.Image != null)
                {
                    if (!string.IsNullOrEmpty(villaToUpdate.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), villaToUpdate.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);

                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    // Tạo tên file bằng cách lấy Id của villa và phần mở rộng của file ảnh
                    string fileName = villaToUpdate.Id + Path.GetExtension(villaDTO.Image.FileName);
                    // Xác định đường dẫn lưu file trong thư mục wwwroot\ProductImage
                    string filePath = @"wwwroot\VillaImage\" + fileName;

                    // Lấy vị trí đầy đủ của file trên hệ thống
                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    
                    // Sử dụng FileStream để tạo file mới và sao chép nội dung ảnh vào
                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        villaDTO.Image.CopyTo(fileStream);
                    }

                    // Tạo URL để truy cập ảnh từ trình duyệt
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    villaToUpdate.ImageUrl = baseUrl + "/VillaImage/" + fileName; // Lưu URL của ảnh vào thuộc tính ImageUrl của villa
                    villaToUpdate.ImageLocalPath = filePath; // Lưu đường dẫn local của ảnh vào thuộc tính ImageLocalPath của villa
                }
                else
                {
                    villaToUpdate.ImageUrl = "https://placehold.co/600x400";
                }
                await _dbVilla.UpdateAsync(villaToUpdate);
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> jsonPatch)
        {
            try
            {
                if (jsonPatch == null || id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(x => x.Id == id, tracked: false);
                var villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);
                if (villa == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                jsonPatch.ApplyTo(villaUpdateDTO, ModelState);
                Villa model = _mapper.Map<Villa>(villaUpdateDTO);
                await _dbVilla.UpdateAsync(model);
                if (!ModelState.IsValid)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(ModelState);
                }
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
