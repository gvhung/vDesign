using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Public.Models
{
    public class CategoryItemVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public string ImageId { get; set; }
        public bool Expand { get; set; }
        public string Color { get; set; }
        public string Mnemonic { get; set; }
    }


}