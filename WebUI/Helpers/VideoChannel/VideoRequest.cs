using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Helpers.VideoChannel
{
    public class VideoRequest
    {
        public int FromId { get; set; }
        public string FromTitle { get; set; }
        public string FromImage { get; set; }
        public string Key { get; set; }
        public string DialogType { get; set; }

        public int ToId { get; set; }
        public string ToTitle { get; set; }
        public string ToImage { get; set; }
        public bool Self { get; set; }

        public int ConferenceId { get; set; }
        public string ConferenceTitle { get; set; }
    }
}