using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Ambient;
using Base.Attributes;
using Base.Security;
using Framework.Attributes;

namespace Base.UI
{
    [EnableFullTextSearch]
    public class DetailViewSetting : BaseObject, IDetailViewSetting
    {
        public DetailViewSetting()
        {
            Fields = new List<FieldSetting>();
        }

        [PropertyDataType("ListWFObjects")]
        [DetailView(Name = "Объект", ReadOnly = true)]
        public string Type { get; set; }

        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true), ListView]
        public string Title { get; set; }

        [DetailView(TabName = "[1]Поля")]
        public virtual ICollection<FieldSetting> Fields { get; set; }

        public void Apply(CommonEditorViewModel commonEditorVm)
        {

        }

        public void Apply(EditorViewModel editor)
        {
            var fieldSetting = Fields.FirstOrDefault(x => x.FieldName == editor.PropertyName);

            if (fieldSetting != null)
            {
                editor.Title = fieldSetting.Title;
                editor.Description = fieldSetting.Description;
                editor.TabName = fieldSetting.TabName;
                editor.IsRequired = fieldSetting.Required;
                editor.Visible = fieldSetting.Visible;
                editor.IsReadOnly = !fieldSetting.Enable;
                editor.IsLabelVisible = !fieldSetting.HideLabel;

                if (fieldSetting.VisibleRoles.Any())
                    editor.Visible = AppContext.SecurityUser.IsAdmin || fieldSetting.VisibleRoles.Any(x => AppContext.SecurityUser.IsRole(x.ID));

                if (fieldSetting.HiddenRoles.Any() && !AppContext.SecurityUser.IsAdmin)
                    editor.Visible = !fieldSetting.HiddenRoles.Any(x => AppContext.SecurityUser.IsRole(x.ID));

                if (fieldSetting.EnableRoles.Any())
                    editor.IsReadOnly = !(AppContext.SecurityUser.IsAdmin || fieldSetting.EnableRoles.Any(x => AppContext.SecurityUser.IsRole(x.ID)));

                if (fieldSetting.ReadOnlyRoles.Any() && !AppContext.SecurityUser.IsAdmin)
                    editor.IsReadOnly = fieldSetting.ReadOnlyRoles.Any(x => AppContext.SecurityUser.IsRole(x.ID));

            }
        }
    }

    public class FieldSetting : BaseObject
    {
        public FieldSetting()
        {

        }

        public FieldSetting(EditorViewModel editorViewModel)
            : this()
        {
            FieldName = editorViewModel.PropertyName;
            Title = editorViewModel.Title;
            Description = editorViewModel.Description;
            TabName = editorViewModel.TabName;
            Required = editorViewModel.IsRequired;
            Visible = editorViewModel.Visible;
            Enable = !editorViewModel.IsReadOnly;
            HideLabel = !editorViewModel.IsLabelVisible;
        }

        [MaxLength(255)]
        [DetailView(Name = "Поле", Required = true)]
        [PropertyDataType("FieldSetting")]
        public string FieldName { get; set; }

        [MaxLength(255)]
        [DetailView(Name = "Наименование", Required = true), ListView]
        public string Title { get; set; }

        [DetailView(Name = "Скрыть наименование")]
        public bool HideLabel { get; set; }

        [DetailView(Name = "Описание"), ListView]
        public string Description { get; set; }

        [MaxLength(255)]
        [DetailView(Name = "Наименование вкладки"), ListView]
        public string TabName { get; set; }

        [DetailView(Name = "Обязательно"), ListView]
        public bool Required { get; set; }

        [DetailView(Name = "Видимо"), ListView]
        public bool Visible { get; set; }

        [DetailView(Name = "Доступно"), ListView]
        public bool Enable { get; set; }

        [DetailView(Name = "Видимо для ролей")]
        public virtual ICollection<FieldRoleVisible> VisibleRoles { get; set; }

        [DetailView(Name = "Скрыто для ролей")]
        public virtual ICollection<FieldRoleHidden> HiddenRoles { get; set; }

        [DetailView(Name = "Доступно для ролей")]
        public virtual ICollection<FieldRoleEnable> EnableRoles { get; set; }

        [DetailView(Name = "Только чтение для ролей")]
        public virtual ICollection<FieldRoleReadOnly> ReadOnlyRoles { get; set; }
    }


    public class FieldRoleVisible : EasyCollectionEntry<Role>
    {

    }

    public class FieldRoleHidden : EasyCollectionEntry<Role>
    {

    }

    public class FieldRoleEnable : EasyCollectionEntry<Role>
    {

    }

    public class FieldRoleReadOnly : EasyCollectionEntry<Role>
    {

    }
}
