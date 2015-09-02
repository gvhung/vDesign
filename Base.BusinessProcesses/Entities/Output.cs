using Base.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    public class Output : BaseObject
    {
        [ListView]
        [DetailView(Name = "Описание")]
        public string Description { get; set; }

        [PropertyDataType("Color")]
        [DetailView(Name = "Цвет")]
        [ListView]
        public string Color { get; set; }

        public int StepID { get; set; }

        public int? NextStepID { get; set; }

        public string NextStepViewID { get; set; }

        public Output()
        {
            Color = "#6f5499";
        }

        public Output(Output src)
        {
            
            Color = src.Color;
            Description = src.Description;
            Color = src.Color;
            NextStepViewID = src.NextStepViewID;
            SystemName = src.SystemName;
            Hidden = src.Hidden;
        }

        [JsonIgnore, InverseProperty("BaseOutputs")]
        public virtual Step Step { get; set; }

        [JsonIgnore]
        public virtual Step NextStep { get; set; }

        [ListView]
        [MaxLength(255)]
        [DetailView(Name = "Служебное имя")]
        public string SystemName { get; set; }  
    }
}