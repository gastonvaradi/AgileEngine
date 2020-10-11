using AgileEngineBEServices.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileEngine_BE.Models
{
    public class ImagesViewModel
    {
        public PictureDetailsDto Search {get;set;}
        public List<AgileEngineBEServices.DTOs.PictureDto> PicturesList { get; set; }
    }
}
