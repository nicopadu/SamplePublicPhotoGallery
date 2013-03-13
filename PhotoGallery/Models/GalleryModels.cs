using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoGallery.Models
{
    public class GalleryImage
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string TumbnailUrl { get; set; }
        public string ImageUrl { get; set; }
    }

}