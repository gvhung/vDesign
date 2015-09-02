using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Helpers.VideoChannel
{
    public class VideoChannel
    {
        public string ChannelKey { get; set; }
        public int DialogId { get; set; }
        public int InitiatorId { get; set; }
        public string DialogType { get; set; }
        public DateTime StartDate { get; set; }
    }
}