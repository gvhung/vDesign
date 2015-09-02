using Base;
using Base.Security;

namespace WebUI.Models
{
    public class GanttResourceViewModel
    {
        public int ID { get; set; }

        public User Employee { get; set; }

        public FileData Image { get; set; }
        public int TasksCount { get; set; }

    }
}