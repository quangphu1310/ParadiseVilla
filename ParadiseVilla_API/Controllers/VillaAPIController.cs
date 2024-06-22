using Microsoft.AspNetCore.Mvc;
using ParadiseVilla_API.Data;
using ParadiseVilla_API.Models;
using ParadiseVilla_API.Models.DTO;

namespace ParadiseVilla_API.Controllers
{
    [Route("/api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return VillaStore.villaList;
        }
        [HttpGet("{id:int}")]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villaStore = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (villaStore == null)
                return NotFound();
            else 
                return Ok(villaStore);
        }
    }
}
