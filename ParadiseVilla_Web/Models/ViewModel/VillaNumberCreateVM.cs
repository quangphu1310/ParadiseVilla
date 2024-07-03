using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParadiseVilla_Web.Models.DTO;

namespace ParadiseVilla_Web.Models.ViewModel
{
    public class VillaNumberCreateVM
    {
        public VillaNumberCreateVM()
        {
            VillaNumberCreateDTO = new VillaNumberCreateDTO();
        }
        public VillaNumberCreateDTO VillaNumberCreateDTO {  get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList {  get; set; } 
    }
}
