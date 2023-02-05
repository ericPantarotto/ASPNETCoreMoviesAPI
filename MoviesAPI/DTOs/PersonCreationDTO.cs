using MoviesAPI.Entities;

namespace MoviesAPI.DTOs
{
    public class PersonCreationDTO : PersonDTOBase, IPictureFormFile, IPictureCustom
    {
        public string PictureCustom { get; set; }
    }
}
