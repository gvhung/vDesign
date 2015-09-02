using Base;
using Base.Attributes;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class ProductConformityDocument : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование")]
        public string Title { get; set; }
        
        [MaxLength(255)]
        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Номер", Required = true)]
        public string Number { get; set; }
        [ListView]
        [DetailView(Name = "Дата выдачи")]
        public DateTime DateIssued { get; set; }

        public int? IssuerID { get; set; }
        [ListView]
        [DetailView(Name = "Кем выдан")]
        public virtual Organization Issuer { get; set; }

        public int? VerifyerID { get; set; }
        [ListView]
        [DetailView(Name = "Кем испытан")]
        public virtual Organization Verifyer { get; set; }

        public int StandartID { get; set; }
        [ListView]
        [DetailView(Name = "Стадндарт")]
        public virtual Standart Standart { get; set; }
    }
}
