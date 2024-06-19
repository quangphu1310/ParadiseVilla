using ParadiseVilla_API.Models.DTO;

namespace ParadiseVilla_API.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>()
        {
            new VillaDTO { Id= 1, Name="Pool View"},
            new VillaDTO { Id= 2, Name="Nice Beach View"}
        };
    }
}
