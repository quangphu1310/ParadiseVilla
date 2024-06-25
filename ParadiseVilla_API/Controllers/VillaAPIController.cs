using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParadiseVilla_API.Data;
using ParadiseVilla_API.Models;
using ParadiseVilla_API.Models.DTO;
using ParadiseVilla_API.Repository.IRepository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ParadiseVilla_API.Controllers
{
    [Route("/api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IVillaRepository _dbVilla;
        private IMapper _mapper;
        public VillaAPIController(IVillaRepository db, IMapper mapper)
        {
            _dbVilla = db;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            var listVilla = await _dbVilla.GetAllAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(listVilla));
        }
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villaStore = await _dbVilla.GetAsync(x => x.Id == id);
            if (villaStore == null)
                return NotFound();
            else
                return Ok(_mapper.Map<VillaDTO>(villaStore));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest();
            }
            if (await _dbVilla.GetAsync(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "The Villa Already Exists!");
                return BadRequest(ModelState);
            }
            Villa villa = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.CreateAsync(villa);
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villaDTO);
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villaToDelete = await _dbVilla.GetAsync(x => x.Id == id);
            if (villaToDelete == null)
            {
                return NotFound();
            }
            await _dbVilla.RemoveAsync(villaToDelete);
            return NoContent();
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villaToUpdate = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.UpdateAsync(villaToUpdate);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> jsonPatch)
        {
            if (jsonPatch == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(x => x.Id == id, tracked:false);
            var villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);
            if (villa == null)
            {
                return NotFound();
            }
            jsonPatch.ApplyTo(villaUpdateDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaUpdateDTO);
            await _dbVilla.UpdateAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
