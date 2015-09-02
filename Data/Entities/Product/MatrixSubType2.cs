using Base;
using Base.Attributes;
using Framework.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class MatrixSubType2 : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }
        [ListView]
        [DetailView(Name = "Полное наименование")]
        public string FullTitle { get; set; }

        public int MatrixTypeID { get; set; }
        [JsonIgnore]
        [DetailView(Name = "Тип матрицы")]
        public virtual MatrixType MatrixType { get; set; }
    }
}