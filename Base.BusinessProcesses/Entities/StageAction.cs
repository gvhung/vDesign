using Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Base.Security;

namespace Base.BusinessProcesses.Entities
{
    [Table("StageActions")]
    public class StageAction : Output
    {
        [ListView]
        [MaxLength(255)]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        public StageAction()
        {
            Color = "#6f5499";
            RequiredComment = false;
        }

        public StageAction(StageAction src)
            : base(src)
        {
            Title = src.Title;
            IsDefaultAction = src.IsDefaultAction;
            RequiredComment = src.RequiredComment;
        
           
        }

        public void CopyInitItems(StageAction src)
        {
            if (src.InitItems != null)
            {
                var iis = new List<StageActionInitItem>();
                iis.AddRange(src.InitItems.Select(ii => new StageActionInitItem(ii)));
                InitItems = iis;
            }
        }

        public void CopyValidationRules(StageAction src)
        {
            if (src.ValidatonRules != null)
            {
                var vrl = new List<StageActionValidationItem>();
                vrl.AddRange(src.ValidatonRules.Select(vr => new StageActionValidationItem() { Property = vr.Property, ValidationRule = vr.ValidationRule }));
                ValidatonRules = vrl;
            }
        }

        [ListView]
        [DetailView(Name = "Действие по-умолчанию")]
        public bool IsDefaultAction { get; set; }

        [PropertyDataType("BPObjectEditButton")]
        [DetailView("Инициализатор объекта", TabName = "Инициализатор объекта")]
        public virtual ICollection<StageActionInitItem> InitItems { get; set; }

        [PropertyDataType("StageValidationEdit")]
        [DetailView("Правила валидации", TabName = "Инициализатор объекта")]
        public virtual ICollection<StageActionValidationItem> ValidatonRules { get; set; }

        public StageAction(TemplateAction actionTemplate)
        {
            if (!String.IsNullOrEmpty(actionTemplate.Color))
                Color = actionTemplate.Color;

            Title = actionTemplate.Title;
        }

        [DetailView(Name = "Коментарий обязателен")]
        public bool RequiredComment { get; set; }

        [DetailView(Name = "Роли")]
        public virtual ICollection<ActionRole> Roles { get; set; }
    }

    public class ActionRole : EasyCollectionEntry<Role>
    {

    }
}
