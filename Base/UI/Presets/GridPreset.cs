using Base.Attributes;
using Base.UI.Service;
using Framework.Maybe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Base.UI.Presets
{
    [Serializable]
    public class GridPreset : Preset
    {
        [DetailView("Кол-во записей")]
        public int PageSize { get; set; }
        
        [DetailView("Список колонок")]
        [PropertyDataType("GridPreset_Columns")]
        public List<ColumnSetting> Columns { get; set; }

        [DetailView("Возможность группировки колонок")]
        public bool Groupable { get; set; }

        public override Preset Init(IUiFasade uiFasade)
        {
            base.Init(uiFasade);

            var columns = uiFasade.GetColumns(OwnerMnemonic)
                .Where(x => x.PropertyInfo.IsDefined(typeof(ListViewAttribute), false)).ToList();

            if (this.Columns == null)
            {
                this.PageSize = OwnerConfig.ListView.DataSource.PageSize;
                this.Groupable = OwnerConfig.ListView.DataSource.Groups.Groupable;

                this.Columns = new List<ColumnSetting>();

                columns.ForEach(col =>
                {
                    var columnSetting = this.Columns.FirstOrDefault(x => x.Name == col.PropertyName);

                    if (columnSetting == null)
                    {
                        columnSetting = new ColumnSetting(col)
                        {
                            Visible = col.Visible,
                            Width = col.Width,
                            SortOrder = col.Order,
                        };

                        columnSetting.ID = this.Columns.Count() + 1;

                        this.Columns.Add(columnSetting.Init(col));
                    }
                });
            }
            else
            {
                bool changeConfig = false;

                var properties = columns.Select(x => x.PropertyName).ToList();

                foreach (var col in this.Columns)
                {
                    var vm = columns.FirstOrDefault(x => x.PropertyName == col.Name);

                    if (vm == null)
                    {
                        changeConfig = true;
                        break;
                    }

                    properties.Remove(vm.PropertyName);

                    col.Init(vm);
                }

                if (changeConfig || properties.Count > 0)
                {
                    this.Columns = null;

                    return this.Init(uiFasade);
                }
            }


            return this;
        }
    }

    [Serializable]
    public class ColumnSetting : BaseObject
    {
        [NonSerialized]
        private ColumnViewModel _viewModel;

        public ColumnSetting() { }

        public ColumnSetting(ColumnViewModel col)
        {
            this.Name = col.PropertyName;
            this.Title = col.Title;
        }

        [JsonIgnore]
        public ColumnViewModel ColumnViewModel { get { return _viewModel; } }
        [JsonIgnore]
        public Type Type { get { return this.ColumnViewModel.With(x => x.PropertyType); } }

        public string Name { get; set; }

        [ListView(Sortable = false)]
        [DetailView("Наименование")]
        [MaxLength(255)]
        public string Title { get; set; }

        [ListView(Sortable = false, Width = 200)]
        [DetailView("Ширина")]
        public int? Width { get; set; }

        [ListView(Sortable = false)]
        [DetailView("Видимость")]
        public bool Visible { get; set; }

        public ColumnSetting Init(ColumnViewModel vm)
        {
            _viewModel = vm;
            return this;
        }
    }
}
