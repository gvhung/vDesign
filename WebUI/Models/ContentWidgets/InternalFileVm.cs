using Base;
using Base.Attributes;
using Base.FileStorage;

namespace WebUI.Models.ContentWidgets
{
    public class InternalFileVm
    {
        [DetailView("Файл", Order = 1)]
        public FileStorageItem File { get; set; }

        [DetailView("Наименование", Order = 0)]
        public string Title { get; set; }
        public InternalFileVm()
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