using System;

namespace WebUI.Models.Forum
{
    public class RecordUserInfo
    {
        public DateTime Date { get; set; }

        public int UserID { get; set; }
        public string UserLogin { get; set; }
        public Guid? Image { get; set; }
    }
}