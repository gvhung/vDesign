using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Public.Models
{
    public class ContentListItemVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public string ImageId { get; set; }
        public int CategoryId { get; set; }
    }

    public class ContentItemVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public string ImageId { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; }
    }

    public class JsonContentListItem
    {
        public string HTMLString { get; set; }
        public bool NoMoreData { get; set; }
    }
}