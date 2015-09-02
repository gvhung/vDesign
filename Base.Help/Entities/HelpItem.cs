using Base.Attributes;
using Base.Security.ObjectAccess;
using Base.UI;
using Framework.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Help.Entities
{
    [EnableFullTextSearch]
    public class HelpItem : HCategory, ITreeNodeIcon, IAccessibleObject
    {
        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual HelpItem Parent_ { get; set; }

        [JsonIgnore]
        public virtual ICollection<HelpItem> Children_ { get; set; }

        #region HCategory
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<HelpItem>(); }
        }

        #endregion

        [DetailView(Name = "Иконка")]
        public Icon Icon { get; set; }

        public HelpItem()
        {
            this.Icon = new Icon();
        }

        [FullTextSearchProperty]
        [DetailView(Name = "Список тэгов", Order = 10)]
        public virtual ICollection<HelpItemTag> Tags { get; set; }

        [DetailView(TabName = "[1]Контент")]
        public Base.Content.Content Content { get; set; }
    }

    [EnableFullTextSearch]
    public class HelpItemTag : BaseObject
    {
        [FullTextSearchProperty]
        [MaxLength(255)]
        [DetailView(Name = "Наименование"), ListView]
        public string Title { get; set; }

        [JsonIgnore]
        //ManyToMany
        public ICollection<HelpItem> HelpItems { get; set; }
    }
}
