using Base;
using Base.Attributes;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomenclature.Entities
{
    [EnableFullTextSearch]
    public class Nomenclature : BaseObject, ICategorizedItem
    {
        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Код", Required = true)]
        [MaxLength(100)]
        public string Сode { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        [MaxLength(255)]
        public string Title { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "Описание")]
        public string Description { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }
        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual NomenclatureCategory Category_ { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.Category_; }
        }
        #endregion
    }
}
