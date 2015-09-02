using Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.ContentWidgets
{
    public class ExternalImageVm : WidgetVm
    {
        [DetailView("URL картинки")]
        [MaxLength(255)]
        public string Url { get; set; }

        [DetailView("Заголовок изображения")]
        [MaxLength(255)]
        public string Title { get; set; }

        [DetailView("Альтернативный текст")]
        [MaxLength(255)]
        public string Alt { get; set; }

        [DetailView("Подпись изображения")]
        [MaxLength(255)]
        public string ImageSignature { get; set; }

        [DetailView("Ширина")]
        public int Width { get; set; }

        public ExternalImageVm()
        {
            this.Width = 300;
        }
    }
}