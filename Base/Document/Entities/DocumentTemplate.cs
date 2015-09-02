using Base.Security.ObjectAccess;
using Framework.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Document.Entities
{
    [EnableFullTextSearch]
    public class DocumentTemplate : DocumentBase, ICategorizedItem, IAccessibleObject
    {
        #region ICategorizedItem
        public int CategoryID { get; set; }
        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual DocumentTemplateCategory Category_ { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.Category_; }
        }
        #endregion
    }
}
