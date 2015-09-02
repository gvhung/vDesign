using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Attributes;
using Framework.Attributes;
using Newtonsoft.Json;
using System.IO;
using Base.Security.ObjectAccess;
using Base.Security.ObjectAccess.Policies;

namespace Base.FileStorage
{

    [AccessPolicy(typeof(CreatorOnly))]
    [EnableFullTextSearch]
    public class FileStorageItem : BaseObject, ICategorizedItem, IAccessibleObject
    {
        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual FileStorageCategory Category_ { get; set; }

        public int? FileID { get; set; }

        [ListView(Order = 0)]
        [DetailView(Name = "Файл")]
        [PropertyDataType("File", Params = "hideSelect")]
        public virtual FileData File { get; set; }

        [ListView(Order = 1)]
        [DetailView(Name = "Наименование", Description = "По умолчанию заполняется наименованием файла")]
        [MaxLength(255)]
        [FullTextSearchProperty]
        public string Title { get; set; }

        [ListView("Расширение", Width = 120, Order = 2)]
        public string Extension { get { return this.File != null ? Path.GetExtension(this.File.FileName).Replace(".", "").ToUpper() : ""; } }

        [ListView("Размер (кб)", Width = 120, Order = 3)]
        public long Size { get { return this.File != null ? this.File.Size / 1024 : 0; } }

        [ListView(Hidden = true)]
        [DetailView(Name = "Описание")]
        [FullTextSearchProperty]
        public string Description { get; set; }


        [DetailView(Name = "Тэги поиска", TabName = "[1]Дополнительно")]
        [FullTextSearchProperty]
        public string Tags { get; set; }

        #region ICategorizedItem
        HCategory ICategorizedItem.Category
        {
            get { return this.Category_; }
        }
        #endregion
    }
}
