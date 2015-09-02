using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex
{
    [ComplexType]
    public class UrlMultilanguageText
    {
        public UrlMultilanguageText()
        {
            Text = new MultilanguageText();
        }

        public MultilanguageText Text { get; set; }
        [MaxLength(255)]
        public string Path { get; set; }
    }
}
