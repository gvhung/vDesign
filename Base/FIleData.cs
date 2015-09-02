using Base.Attributes;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base
{
    [EnableFullTextSearch]
    [Table("Files")]
    [Serializable]
    public class FileData : BaseObject
    {
        public FileData()
        {
            CreationDate = DateTime.Now;
            ChangeDate = DateTime.Now;
        }

        [SystemProperty]
        public Guid FileID { get; set; }

        [SystemProperty]
        [MaxLength(255)]
        [DetailView(Name = "Имя файла", Order = 0)]
        [FullTextSearchProperty]
        [ListView]
        public string FileName { get; set; }

        [SystemProperty]
        [DetailView(Name = "Размер", Order = 1, ReadOnly = true)]
        [ListView]
        public long Size { get; set; }

        [SystemProperty]
        [DetailView(Name = "Дата создания", Order = 2, ReadOnly = true)]
        public DateTime CreationDate { get; set; }

        [SystemProperty]
        [DetailView(Name = "Дата последнего изменения", Order = 3, ReadOnly = true)]
        public DateTime ChangeDate { get; set; }

        //TODO: убрать!!!
        public Guid Key
        {
            get { return FileID; }
        }
    }
}
