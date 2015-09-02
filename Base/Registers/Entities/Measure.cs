using Base.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Registers.Entities
{
    public class Measure : BaseObject, ICategorizedItem
    {
        [ListView]
        [MaxLength(10)]
        [DetailView(Name = "Код", Required = true)]
        public string Code { get; set; }

        [ListView]
        [MaxLength(255)]
        [DetailView(Name = "Условное обозначение", Required = true)]
        public string Title { get; set; }

        [DetailView(Name = "Описание")]
        [ListView]
        public string Description { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual MeasureCategory Category_ { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.Category_; }
        }
        #endregion
    }
}
