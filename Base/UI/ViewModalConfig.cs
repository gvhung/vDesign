using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Base.UI
{
    [DebuggerDisplay("Mnemonic={Mnemonic}")]
    public class ViewModelConfig
    {
        public string Mnemonic { get; private set; }
        public string Title { get; private set; }
        public string Service { get; private set; }
        public string Entity { get; private set; }
        public ListView ListView { get; private set; }
        public DetailView DetailView { get; private set; }
        public string LookupProperty { get; private set; }
        public bool IsReadOnly { get; private set; }

        public string LookupPropertyForUI
        {
            get
            {
                Type typeLookupProperty = this.TypeEntity.GetProperty(this.LookupProperty).PropertyType;

                if (typeof(Base.Entities.Complex.MultilanguageText).IsAssignableFrom(typeLookupProperty))
                {
                    return this.LookupProperty + ".Lang.ru";
                }

                return this.LookupProperty;
            }
        }

        public string LookupPropertyForFilter
        {
            get
            {
                Type typeLookupProperty = this.TypeEntity.GetProperty(this.LookupProperty).PropertyType;

                if (typeof(Base.Entities.Complex.MultilanguageText).IsAssignableFrom(typeLookupProperty))
                {
                    return this.LookupProperty + ".Xml";
                }

                return this.LookupProperty;
            }
        }

        public string Icon { get; private set; }

        private Type _typeService;
        public Type TypeService
        {
            get
            {
                if (_typeService == null && !String.IsNullOrEmpty(this.Service))
                {
                    _typeService = Type.GetType(this.Service);
                }

                return _typeService;
            }
        }

        private Type _typeEntity;
        public Type TypeEntity
        {
            get
            {
                if (_typeEntity == null && !String.IsNullOrEmpty(this.Entity))
                {
                    _typeEntity = Type.GetType(this.Entity);
                }

                return _typeEntity;
            }
        }

        public ViewModelConfig(string mnemonic, string entity, ListView listView, DetailView detailView, string service = "", string title = "", string lookupProperty = "", string icon = "", bool IsReadOnly = false, Type entityType = null)
        {
            _typeEntity = entityType;
            this.Mnemonic = mnemonic;
            this.Title = String.IsNullOrEmpty(title) ? mnemonic : title;
            this.Service = service;
            this.Entity = entity;
            this.ListView = listView;
            this.DetailView = detailView;
            this.LookupProperty = lookupProperty;
            this.Icon = icon;
            this.IsReadOnly = IsReadOnly;
        }
    }

    public class ListView
    {
        public ListView()
        {
            this.AutoRefreshInterval = -1;
            Columns = new List<Column>();
        }

        public ListViewType Type { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public bool Sortable { get; set; }
        public bool Scrollable { get; set; }
        public DataSource DataSource { get; set; }
        public bool HideToolbar { get; set; }
        public List<Action> HiddenActions { get; set; }
        public List<Toolbar> Toolbars { get; set; }
        public List<Column> Columns { get; set; }
        public List<ConditionalAppearance> ConditionalAppearance { get; set; }
        public string CSHtmlHelper { get; set; }
        public int AutoRefreshInterval { get; set; }
        public string CustomDialog { get; set; }
        public bool ShowPreview { get; set; }
        public bool AllowEmptyAction { get; set; }
    }

    public class ListViewCategorizedItem : ListView
    {
        public string MnemonicCategory { get; set; }
        public bool HiddenTree { get; set; }
    }

    public class TreeView : ListView
    {
        public bool ExtendedCategory { get; set; }
    }

    public enum ListViewType
    {
        Grid = 0,
        Tree = 1,
        Scheduler = 2,
        Gantt = 3,
        Custom = 4
    }

    public class DataSource
    {
        public string SystemFilter { get; set; }
        public bool ServerOperation { get; set; }
        public DataSourceAction Get { get; set; }
        public int PageSize { get; set; }
        public List<Sort> Sorts { get; set; }
        public List<Filter> Filters { get; set; }
        public GroupCollection Groups { get; set; }
        public bool ExecuteQueryBeforeApplyClientFilter { get; set; }

        public DataSource()
        {
            this.Groups = new GroupCollection();
        }
    }

    public class Sort
    {
        public string Descriptor { get; set; }
        public string Order { get; set; }
    }

    public class Filter
    {
        public int Group { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class GroupCollection : IEnumerable<Group>
    {
        public bool Groupable { get; set; }
        public IList<Group> Groups { get; set; }

        public GroupCollection()
        {
            this.Groups = new List<Group>();
        }

        public Group this[string field]
        {
            get { return this.Groups.FirstOrDefault(x => x.Field == field); }
        }

        public IEnumerator<Group> GetEnumerator()
        {
            return this.Groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Group
    {
        public string Field { get; set; }
    }

    public interface IDetailView
    {
        string Title { get; set; }
        string Name { get; set; }
        int? Width { get; set; }
        int? Height { get; set; }
        bool isMaximaze { get; set; }
        bool HideToolbar { get; set; }
        DetailViewDataSource DataSource { get; set; }
        List<Toolbar> Toolbars { get; set; }
        List<Editor> Editors { get; set; }
        string CSHtmlHelper { get; set; }
        string WizardName { get; set; }
        AjaxFormAction AjaxForm { get; set; }
    }

    public abstract class BaseDetailView: IDetailView
    {
        protected BaseDetailView()
        {
            Editors = new List<Editor>();
        }

        public string Title { get; set; }
        public string Name { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool isMaximaze { get; set; }
        public bool HideToolbar { get; set; }
        public DetailViewDataSource DataSource { get; set; }
        public List<Toolbar> Toolbars { get; set; }
        public List<Editor> Editors { get; set; }
        public string CSHtmlHelper { get; set; }
        public string WizardName { get; set; }
        public AjaxFormAction AjaxForm { get; set; }
        public bool HideTab { get; set; }
    }

    public class DetailView : BaseDetailView
    {
    }

    public class DetailViewDataSource
    {
        public DataSourceAction Get { get; set; }
        public DataSourceAction Save { get; set; }
    }

    public class DataSourceAction
    {
        public string Name { get; set; }
        public string Controller { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }

    public class AjaxFormAction
    {
        public string Name { get; set; }
        public string Controller { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }

    public class Action
    {
        public string ID;
    }

    public class ConditionalAppearance
    {
        public string Condition { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public string Backgound { get; set; }
    }

    public class Toolbar
    {
        public Guid ToolbarID { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string Icon { get; set; }
        public Dictionary<string, string> Params { get; set; }
        public bool IsAjax { get; set; }
        public string Url { get; set; }
        public bool IsMaximize { get; set; }
        public bool OnlyForSelected { get; set; }
        public Toolbar()
        {
            this.Params = new Dictionary<string, string>();

            this.ToolbarID = Guid.NewGuid();

            this.OnlyForSelected = true;
        }
    }

    public class Editor
    {
        public Editor(string name)
        {
            this.PropertyName = name;
        }

        public string PropertyName { get; protected set; }
        public string Mnemonic { get; set; }
        public string Title { get; set; }
        public bool? IsLabelVisible { get; set; }
        public string EditorTemplate { get; set; }
        public string TabName { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? IsRequired { get; set; }
        public bool? Visible { get; set; }
        public int? Order { get; set; }
    }

    public class Column
    {
        public Column(string name)
        {
            this.PropertyName = name;
        }

        public string PropertyName { get; protected set; }
        public string Mnemonic { get; set; }
        public string Title { get; set; }
        public bool? Hidden { get; set; }
        public bool Visible { get { return !(Hidden ?? false); } set { this.Hidden = !value; } }
        public bool? Filterable { get; set; }
        public int? Order { get; set; }
        public bool? Locked { get; set; }
        public bool? Lockable { get; set; }
    }
}
