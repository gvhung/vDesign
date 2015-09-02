using Base.BusinessProcesses.Entities;
using Base.UI;

namespace WebUI.Models.BusinessProcess
{
    public class CreatedObjectVm
    {
        public int ID { get; set; }
        public ViewModelConfig Config { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }


        public CreatedObjectVm(ViewModelConfig config, CreatedObject obj)
        {
            this.ID = obj.ObjectID;
            this.Config = config;
            this.Description = obj.ObjectStep.Description;
            this.Title = obj.ObjectStep.Title;
        }
    }
}