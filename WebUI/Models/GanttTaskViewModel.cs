using Base.Task.Entities;
using Kendo.Mvc.UI;
using System;

namespace WebUI.Models
{
    public class GanttTaskViewModel : IGanttTask
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string AllParents { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime End { get; set; }
        public bool Expanded { get; set; }
        public int OrderId { get; set; }
        public decimal PercentComplete { get; set; }
        public DateTime Start { get; set; }
        public bool Summary { get; set; }
        public string Title { get; set; }
        public Priority Priority { get; set; }
        public String Highlight { get; set; }

    }
}