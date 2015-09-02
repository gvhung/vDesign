using Base.Attributes;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base
{
    [EnableFullTextSearch]
    public abstract class HCategory : BaseObject, ITreeNode
    {
        public const char Seperator = ';';
        
        [JsonIgnore]
        public string sys_all_parents { get; protected set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Order = 1, Required = true)]
        [MaxLength(500)]
        public virtual string Name { get; set; }

        public int? ParentID { get; set; }

        [JsonIgnore]
        [NotMapped]
        public abstract HCategory Parent { get; }
        [JsonIgnore]
        [NotMapped]
        public abstract IEnumerable<HCategory> Children { get; }

        [NotMapped]
        public int Level 
        {
            get
            {
                if (this.sys_all_parents == null)
                    return 0;

                return this.sys_all_parents.Split(HCategory.Seperator).Count();
            }
        }

        [NotMapped]
        public bool IsRoot
        {
            get { return this.Level == 0; }
        }

        public int GetParentID(int level)
        {
            if (this.sys_all_parents == null)
                return -1;

            string parentID = this.sys_all_parents.Split(HCategory.Seperator).Skip(level).FirstOrDefault();

            if (parentID == null)
                return -1;

            return HCategory.IdToInt(parentID);
        }

        #region Helpers
        public static string IdToString(int id)
        {
            return String.Format("@{0}@", id);
        }

        public static int IdToInt(string id)
        {
            return Int32.Parse(id.Replace("@", "").Replace("@", ""));
        }

        public void SetParent(HCategory parent)
        {
            if (parent == null)
            {
                this.ParentID = null;
                this.sys_all_parents = null;
            }
            else
            {
                if (this.ID == parent.ID)
                {
                    throw new Exception("Циклическая зависимость");
                }

                if (parent.sys_all_parents != null && parent.sys_all_parents.Contains(HCategory.IdToString(this.ID)))
                {
                    throw new Exception("Циклическая зависимость");
                }

                this.ParentID = parent.ID;
                this.sys_all_parents = (parent.sys_all_parents != null ? parent.sys_all_parents + Seperator : "") + IdToString(parent.ID);
            }
        }
        #endregion

        #region ITreeNode
        string ITreeNode.Name
        {
            get { return this.Name; }
        }

        ITreeNode ITreeNode.Parent
        {
            get { return this.Parent; }
        }

        IEnumerable<ITreeNode> ITreeNode.Children
        {
            get { return this.Children; }
        }
        #endregion
    }
}
