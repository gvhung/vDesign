namespace Base.Registers.Entities
{
    using Base.Attributes;
    using Framework.Attributes;
    using System.ComponentModel.DataAnnotations;

    [EnableFullTextSearch]
    public class Country : BaseObject
    {
        [FullTextSearchProperty]
        [DetailView(Name = "2-буквенный код", Required = true, Order = 0)]
        [ListView]
        [MaxLength(3)]
        public string Alpha2Code { get; set; }

        [DetailView(Name = "Цифровой код", Required = true, Order = 1)]
        [ListView]
        public int NumericCode { get; set; }

        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true, Order = 2)]
        [ListView]
        public string Title { get; set; }
    }
}