using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Models.ViewModel
{
    public class VillaNumberDeleteVM
    {
        public VillaNumberDeleteVM()
        {
            VillaNumberDTO = new VillaNumberDTO();
        }
        public VillaNumberDTO VillaNumberDTO {  get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList {  get; set; } 
    }
}
