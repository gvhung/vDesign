using Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.ContentWidgets
{
    public class InternalImageVm : InternalFileVm
    {
        [DetailView("Оригинал (приоритетный параметр)", TabName = "Дополнительно")]
        public bool Original { get; set; }

        [DetailView("Ширина", TabName = "Дополнительно")]
        public int Width { get; set; }

        [DetailView("Высота", TabName = "Дополнительно")]
        public int Height { get; set; }

        [DetailView("Заголовок изображения", Order = 2)]
        [MaxLength(255)]
        public string Title { get; set; }

        [DetailView("Альтернативный текст", Order = 2)]
        [MaxLength(255)]
        public string Alt { get; set; }

        [DetailView("Подпись изображения", Order = 2)]
        [MaxLength(255)]
        public string ImageSignature { get; set; }

        public InternalImageVm()
        {
            this.Original = true;
            this.Height = 300;
            this.Width = 300;
        }
    }
}