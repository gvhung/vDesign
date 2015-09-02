using Base;
using Base.Attributes;
using Base.FileStorage;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.ContentWidgets
{
    public class VideoVm : WidgetVm
    {
    }

    public class ExternalVideoVm : WidgetVm
    {
        [DetailView("Ссылка")]
        [MaxLength(int.MaxValue)]
        public string Url { get; set; }
    }

    public class InternalVideoVm : InternalFileVm
    {
        public InternalVideoVm()
        {
            this.File = new FileStorageItem
            {
                File = new FileData
                {
                    FileName = "test.png"
                }
            };
        }
    }
}