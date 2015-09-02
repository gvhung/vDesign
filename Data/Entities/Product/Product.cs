using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.Nomenclature.Entities;
using Data.Entities.Workgroup;
using Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class Product : BaseObject, IBPObject
    {
        public Product()
        {
            Okpd = new Okpd();
        }

        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        [ListView]
        [DetailView(Name = "Описание")]
        public string Description { get; set; }

        [ListView]
        [DetailView(Name = "Класс продукта", Required = true)]
        public virtual ProductClass ProductClass { get; set; }

        public int? MatrixTypeID { get; set; }
        [DetailView(Name = "Тип матрицы", Visible = false)]
        public virtual MatrixType MatrixType { get; set; }

        public int? MatrixSubtypeID { get; set; }


        [DetailView(Name = "Подтип матрицы", Visible = false)]
        public virtual MatrixSubType MatrixSubType { get; set; }


        public int? FillerClassID { get; set; }
        [DetailView(Name = "Класс наполнителя", Visible = false)]
        public virtual FillerClass FillerClass { get; set; }

        public int? FillerSubClassID { get; set; }
        [DetailView(Name = "Подкласс наполнителя", Visible = false)]
        public virtual FillerSubClass FillerSubClass { get; set; }

        public int? FillerTypeID { get; set; }
        [DetailView(Name = "Тип наполнителя", Visible = false)]
        public virtual FillerType FillerType { get; set; }

        public int? SoftwareTypeID { get; set; }
        [DetailView(Name = "Тип ПО", Visible = false)]
        public virtual SoftwareType SoftwareType { get; set; }

        [DetailView(Name = "Возможности ПО", Visible = false)]
        public virtual ICollection<SoftwareFeature> SoftwareFeatures { get; set; }


        [PropertyDataType("InitWorkflow")]
        [NotMapped]
        [DetailView("Шаблон бизнес-процесса", Visible = false)]
        public Workflow InitWorkflow { get; set; }

        //public int CompositeMaterialID { get; set; }
        //public virtual CompositeMaterial CompositeMaterial { get; set; }
        [DetailView(Name = "ОКПД", Visible = false)]
        public Okpd Okpd { get; set; }

        public int? TechID { get; set; }
        [DetailView(Name = "Производственная технология", Visible = false)]
        public virtual Tech Tech { get; set; }

        [DetailView(Name = "Места применения", Visible = false)]
        public virtual ICollection<ApplicationSummary> ApplicationAreas { get; set; }

        [DetailView(Name = "Сертификаты соответствия", Visible = false)]
        public virtual ICollection<ProductConformityDocument> ProductConformityDocuments { get; set; }

        public int? ProductionStageID { get; set; }
        [DetailView(Name = "Этап производственной цепочки", Visible = false)]
        public virtual ProductionStage ProductionStage { get; set; }

        public int? AdditiveTypeID { get; set; }
        [DetailView(Name = "Область применения добавки", Visible = false)]
        public virtual AdditiveType AdditiveType { get; set; }

        public int? AdditiveSubTypeID { get; set; }
        [DetailView(Name = "Назначение добавки", Visible = false)]
        public virtual AdditiveSubType AdditiveSubType { get; set; }


        public int? CompositeTypeID { get; set; }
        [DetailView(Name = "Тип композита", Visible = false)]
        public virtual CompositeType CompositeType { get; set; }

        public int? CompositeSubTypeID { get; set; }
        [DetailView(Name = "Подтип композита", Visible = false)]
        public virtual CompositeSubType CompositeSubType { get; set; }

        [DetailView(Name = "Добавки в КМ", Visible = false)]
        public virtual ICollection<Product> Additives { get; set; }

        public int? FillerID { get; set; }
        [DetailView(Name = "Наполнитель КМ", Visible = false)]
        public virtual Product Filler { get; set; }

        public int? MatrixID { get; set; }
        [DetailView(Name = "Матрица КМ", Visible = false)]
        public virtual Product Matrix { get; set; }

        [DetailView(TabName = "Файлы", Visible = false)]
        public virtual ICollection<ProductFile> Files { get; set; }

        [DetailView(Name = "текстовое поле запроса на", Visible = false)]
        public string Request { get; set; }

        public int? WorkGroupID { get; set; }
        [DetailView("Рабочая группа", Visible = false)]
        public virtual WorkGroup WorkGroup { get; set; }

        public int? ManufacturingID { get; set; }
        [ListView]
        [DetailView(Name = "Производственная площадка", Visible = false)]
        public virtual Manufacturing Manufacturing { get; set; }

        [DetailView(Name = "Используемое оборудование", Visible = false)]
        public virtual ICollection<Product> Equipments { get; set; }

        public int? WorkflowContextID { get; set; }

        [DetailView(Name = "Контекст исполнения", Visible = false)]
        public virtual WorkflowContext WorkflowContext { get; set; }
    }

    public enum ProductClass
    {
        [Description("Материалы матрицы")]
        MatrixMaterial = 0,
        [Description("Армирующие наполнители")]
        ReinforcingFillers = 1,
        [Description("Добавки и вспомогательные материалы")]
        AdditiveAuxiliaryMaterials = 2,
        [Description("КМ и полуфабрикаты")]
        KmSemifinished = 3,
        [Description("Оборудование")]
        Equipment = 4,
        [Description("Программное обеспечение")]
        Software = 5,
    }

    [Table("ProductFiles")]
    public class ProductFile : FileData
    {
        public int ProductID { get; set; }
    }


}
