using Base;
using Base.UI;
using Base.Wizard;
using Base.Wizard.UI;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Xml.Linq;
using Group = Base.UI.Group;
using GroupCollection = Base.UI.GroupCollection;
using ListView = Base.UI.ListView;
using TreeView = Base.UI.TreeView;

namespace WebUI.Concrete
{
    public class ViewModelConfigLoader : IViewModelConfigLoader
    {
        public List<ViewModelConfig> Load(Func<Type, ViewModelConfig> defViewModelConfig)
        {
            var dir = new DirectoryInfo(HostingEnvironment.MapPath("/App_Data"));

            var configs = new List<ViewModelConfig>();

            foreach (string fileName in dir.GetFiles("*ViewModelConfig*.xml").Select(x => x.FullName))
            {
                LoadFile(configs, fileName, defViewModelConfig);
            }

            return configs;
        }

        private void LoadFile(List<ViewModelConfig> configs, string fileName, Func<Type, ViewModelConfig> defViewModelConfig)
        {
            XDocument document = XDocument.Load(fileName);
            var mergedElements = new List<XElement>();

            if (document != null)
            {
                XElement root = document.Element("ViewModelConfig");

                string defaultIcon = root.Attribute("defaulticon") != null ? root.Attribute("defaulticon").Value : "";

                if (root != null)
                {
                    List<string> defaultLookupPropertyList = new List<string>() { "Title", "Name", "FirstName", "Lookup" };

                    foreach (XElement xItem in root.Elements("item"))
                    {
                        var item = xItem;

                        if (item.Attribute("base") != null)
                        {
                            var baseXElement =
                                mergedElements
                                    .FirstOrDefault(
                                        x => x.Element("mnemonic").With(b => b.Value) == item.Attribute("base").Value);

                            if (baseXElement == null)
                            {
                                baseXElement = root.Elements("item")
                                    .FirstOrDefault(
                                        x => x.Element("mnemonic").With(b => b.Value) == item.Attribute("base").Value);
                            }

                            if (baseXElement == null)
                                throw new Exception("Base mnemonic was not found");

                            mergedElements.Add(item = MergeXElements(item, baseXElement));
                        }

                        XElement mnemonic = item.Element("mnemonic");
                        XElement service = item.Element("service");
                        XElement entity = item.Element("entity");

                        XElement title = item.Element("title");
                        XElement listView = item.Element("listview");
                        XElement detailView = item.Element("detailview");
                        XElement lookupProperty = item.Element("lookupproperty");
                        XElement icon = item.Element("icon");
                        XElement isreadonly = item.Element("readonly");

                        if (mnemonic != null && entity != null)
                        {
                            string stitle = title != null ? title.Value : "";

                            if (configs.Any(x => x.Mnemonic == mnemonic.Value))
                            {
                                throw new Exception(String.Format("Config already has mnemonic \"{0}\"", mnemonic.Value));
                            }


                            entity.Value = Regex.Replace(entity.Value, "(\\[.*\\])", match => string.Format("`1[{0}]", match.Value));
                            Type entityType = Type.GetType(entity.Value);

                            if (entityType == null)
                            {
                                throw new Exception(String.Format("Type for entity can not be found by string \"{0}\"", entity.Value));
                            }

                            var defaultViewModelConfig = defViewModelConfig(entityType);

                            var wizType =
                                entityType.TypeHierarchy()
                                    .FirstOrDefault(
                                        x =>
                                            x.IsGenericType &&
                                            x.GetGenericTypeDefinition() == typeof(DecoratedWizardObject<>));

                            if (wizType != null)
                            {
                                var decoratedType = wizType.GetGenericArguments()[0];
                                if (decoratedType != null)
                                {

                                    Type configuratorType = typeof(DecoratorConfiguration<>).MakeGenericType(decoratedType);
                                    dynamic configurator = Activator.CreateInstance(configuratorType);
                                    dynamic wizardIntance = Activator.CreateInstance(entityType);
                                    wizardIntance.Configure(configurator);
                                    List<PropertyInfo> properties = configurator.Properties;

                                    var dynamicType = Framework.Emit.DecoratorBuilder.CreateDynamicDecorator(decoratedType, properties,
                                        entityType, typeName: string.Format("{0}.{1}_{2}", entityType.Namespace, entityType.Name, decoratedType.Name));

                                    entityType = dynamicType;
                                }

                            }

                            string serviceName = service.With(x => x.Value, "");

                            if (!String.IsNullOrEmpty(serviceName))
                            {
                                serviceName = Regex.Replace(serviceName, "(\\[.*\\])",
                                    match => string.Format("`1[{0}]", match.Value));

                                var serviceType = Type.GetType(serviceName);

                                if (serviceType == null)
                                {
                                    throw new Exception(
                                        String.Format("Type for service can not be found by string \"{0}\"",
                                            serviceName));
                                }
                            }
                            else if (defaultViewModelConfig != null)
                            {
                                serviceName = defaultViewModelConfig.Service;
                            }

                            var typeProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance /*| BindingFlags.IgnoreCase*/).Select(x => x.Name);

                            string lookup = null;

                            if (lookupProperty != null)
                            {
                                lookup = typeProperties.FirstOrDefault(x => x == lookupProperty.Value);
                            }

                            if (String.IsNullOrEmpty(lookup))
                            {
                                lookup = typeProperties.Intersect(defaultLookupPropertyList).FirstOrDefault() ?? "ID";
                            }

                            string lname = null;

                            if (listView != null)
                            {
                                lname = listView.Attribute("name") != null ? listView.Attribute("name").Value : listView.Value;
                            }

                            string listType = this.GetAttrValue(listView, "type", null);

                            if (listType == null)
                            {
                                if (typeof(ITreeNode).IsAssignableFrom(entityType))
                                {
                                    listType = "Tree";
                                }
                                else if (typeof(IScheduler).IsAssignableFrom(entityType))
                                {
                                    listType = "Scheduler";
                                }
                                else
                                {
                                    listType = "Grid";
                                }
                            }

                            ListView objListView;

                            if (entityType.GetInterfaces().Contains(typeof(ICategorizedItem)))
                            {
                                Type categoryType = entityType.GetProperties().FirstOrDefault(pr => pr.PropertyType.GetInterfaces().Contains(typeof(ITreeNode))).PropertyType;

                                objListView = new ListViewCategorizedItem()
                                {
                                    MnemonicCategory = this.GetAttrValue(listView, "MnemonicCategory", categoryType.FullName),
                                    HiddenTree = this.GetAttrValue(listView, "hiddenTree", false)
                                };
                            }
                            else if (entityType.GetInterfaces().Contains(typeof(ITreeNode)))
                            {
                                objListView = new TreeView()
                                {
                                    ExtendedCategory = this.GetAttrValue(listView, "extendedCategory", typeof(IExtendedCategory).IsAssignableFrom(entityType))
                                };
                            }
                            else
                            {
                                objListView = new ListView();
                            }

                            objListView.Title = this.GetAttrValue(listView, "title", stitle);
                            objListView.Type = (ListViewType)Enum.Parse(typeof(ListViewType), listType);
                            objListView.Name = lname;
                            objListView.HiddenActions = this.GetHiddenActions(listView);
                            objListView.Toolbars = this.GetToolbars(listView);
                            objListView.DataSource = GetDataSource(listView);
                            objListView.Columns = GetColumns(listView);
                            objListView.ConditionalAppearance = GetConditionalAppearance(listView);
                            objListView.Sortable = this.GetAttrValue(listView, "sortable", true);
                            objListView.Scrollable = this.GetAttrValue(listView, "scrollable", true);
                            objListView.HideToolbar = this.GetAttrValue(listView, "hideToolbar", false);
                            objListView.ShowPreview = this.GetAttrValue(listView, "showPreview", false);
                            objListView.CSHtmlHelper = this.GetAttrValue(listView, "CSHtmlHelper", null);
                            objListView.AutoRefreshInterval = this.GetAttrValue(listView, "AutoRefreshInterval", -1);



                            if (listType == "Custom")
                            {
                                objListView.CustomDialog = this.GetAttrValue(listView, "dialog", null);
                            }

                            string dwname = null;
                            int? detailView_W = null;
                            int? detailView_H = null;
                            string wizardName = null;

                            if (detailView != null)
                            {
                                dwname = detailView.Attribute("name") != null ? detailView.Attribute("name").Value : detailView.Value;
                                detailView_W = detailView.Attribute("width") != null ? (int?)Int32.Parse(detailView.Attribute("width").Value) : null;
                                detailView_H = detailView.Attribute("height") != null ? (int?)Int32.Parse(detailView.Attribute("height").Value) : null;
                                wizardName = detailView.Attribute("wizard") != null ? detailView.Attribute("wizard").Value : string.Empty;
                            }

                            var objDetailView = new DetailView()
                            {
                                Title = this.GetAttrValue(detailView, "title", stitle),
                                Name = dwname,
                                Width = detailView_W,
                                Height = detailView_H,
                                isMaximaze = this.GetAttrValue(detailView, "isMaximaze", false),
                                HideToolbar = this.GetAttrValue(detailView, "hideToolbar", false),
                                DataSource = this.GetDetailViewDataSource(detailView),
                                Toolbars = this.GetToolbars(detailView),
                                Editors = this.GetEditors(detailView),
                                CSHtmlHelper = this.GetAttrValue(detailView, "CSHtmlHelper", null),
                                WizardName = wizardName,
                                AjaxForm = this.GetAjaxFormAction(detailView),
                                HideTab = this.GetAttrValue(detailView, "hideTab", false),
                            };


                            if (wizType != null)
                            {
                                objDetailView = new WizardDetailView(objDetailView)
                                {
                                    Name = detailView.Attribute("name") != null ? detailView.Attribute("name").Value : null,
                                    Steps = GetSteps(detailView, objDetailView.Editors),
                                    FirstStep = detailView.Attribute("first") != null ? detailView.Attribute("first").Value : string.Empty,
                                    CompleteText = detailView.Element("completetext") != null ? detailView.Element("completetext").Value : string.Empty
                                };
                            }

                            configs.Add(new ViewModelConfig(mnemonic.Value, entity.Value, objListView, objDetailView,
                                 serviceName,
                                 stitle,
                                 lookup,
                                 icon != null ? icon.Value : defaultIcon,
                                 isreadonly != null && isreadonly.Value.ToUpper() == "TRUE", entityType: entityType));
                        }
                    }
                }
            }
        }

        private XElement MergeXElements(XElement item, XElement baseElement)
        {
            var mergeChildElements = MergeChildElements(baseElement, item);
            return mergeChildElements;
        }

        static XElement MergeChildElements(XElement mergedElement, XElement element)
        {
            if (mergedElement == null)
                return element;

            if (mergedElement.Name == "hiddenActions")
                return new XElement(element.Name, element.Attributes(), mergedElement.Elements(), element.Elements());

            XElement newMergedElement = new XElement(element.Name,
                element.Attributes(),
                element.Elements().Select(e =>
                {
                    if (e.Name == "mnemonic" || e.Name == "title" || e.Name == "service" || e.Name == "entity" ||
                        e.Name == "icon")
                        return e;

                    // only &&!
                    if (e.Name == "systemfilter")
                    {
                        var old = mergedElement.Elements(e.Name).Attributes().FirstOrDefault(x => x.Name == "value");
                        if (old != null)
                        {
                            var newVal = new XAttribute("value", e.Attribute("value").Value + " and " + old.Value);

                            return new XElement(e.Name, newVal);
                        }

                        return e;
                    }

                    //if (e.Name == "merge attrs")
                    //    return new XElement(e.Name,
                    //        e.Attributes(),
                    //        mergedElement.Elements(e.Name).Attributes()
                    //            .Where(a => !(e.Attributes().Any(z => z.Name == a.Name))));

                    XElement correspondingElement = mergedElement.Element(e.Name);
                    if (correspondingElement == null)
                        return e;

                    return new XElement(e.Name,
                        e.Attributes(),
                        e.Elements().Select(c =>
                            MergeChildElements(correspondingElement.Element(c.Name), c)),
                        correspondingElement.Elements().Where(m => e.Element(m.Name) == null));
                }),
                mergedElement.Elements()
                    .Where(m => !element.Elements(m.Name).Any()));

            return newMergedElement;
        }

        private DetailViewDataSource GetDetailViewDataSource(XElement node)
        {
            if (node != null)
            {
                XElement xDataSource = node.Element("dataSource");

                if (xDataSource != null)
                {
                    DetailViewDataSource ds = new DetailViewDataSource();

                    foreach (XElement el in xDataSource.Elements())
                    {
                        if (el.Name == "get" || el.Name == "save")
                        {
                            DataSourceAction action = new DataSourceAction()
                            {
                                Name = this.GetAttrValue(el, "action"),
                                Controller = this.GetAttrValue(el, "controller"),
                            };

                            action.Params = new Dictionary<string, string>();

                            foreach (XElement xParam in el.Elements())
                            {
                                action.Params.Add(this.GetAttrValue(xParam, "key"), this.GetAttrValue(xParam, "value"));
                            }

                            if (el.Name == "get")
                                ds.Get = action;
                            else
                                ds.Save = action;
                        }
                    }

                    return ds;
                }
            }

            return null;
        }

        private AjaxFormAction GetAjaxFormAction(XElement node)
        {
            if (node != null)
            {
                XElement xAjaxFormAction = node.Element("ajaxform");

                if (xAjaxFormAction != null)
                {
                    var action = new AjaxFormAction()
                    {
                        Name = this.GetAttrValue(xAjaxFormAction, "action"),
                        Controller = this.GetAttrValue(xAjaxFormAction, "controller"),
                    };


                    action.Params = new Dictionary<string, string>();

                    foreach (XElement xParam in xAjaxFormAction.Elements())
                    {
                        action.Params.Add(this.GetAttrValue(xParam, "key"), this.GetAttrValue(xParam, "value"));
                    }

                    return action;
                }
            }

            return null;
        }


        private DataSource GetDataSource(XElement node)
        {

            if (node != null)
            {
                XElement xDataSource = node.Element("dataSource");

                if (xDataSource != null)
                {
                    XElement xSorts = xDataSource.Element("sorts");

                    List<Sort> sorts = new List<Sort>();

                    if (xSorts != null)
                    {
                        foreach (XElement xSort in xSorts.Elements())
                        {
                            sorts.Add(new Sort
                            {
                                Descriptor = GetAttrValue(xSort, "descriptor", null),
                                Order = GetAttrValue(xSort, "order", "asc")
                            });
                        }
                    }

                    XElement xFilters = xDataSource.Element("filters");

                    List<Filter> filters = new List<Filter>();

                    if (xFilters != null)
                    {
                        foreach (XElement xFilter in xFilters.Elements())
                        {
                            filters.Add(new Filter
                            {
                                Group = Int32.Parse(GetAttrValue(xFilter, "group", "0")),
                                Field = GetAttrValue(xFilter, "field", null),
                                Operator = GetAttrValue(xFilter, "operator", "IsEqualTo"),
                                Value = GetAttrValue(xFilter, "value", null),
                            });
                        }
                    }

                    GroupCollection gc = new GroupCollection();

                    XElement xGroups = xDataSource.Element("groups");

                    if (xGroups != null)
                    {
                        List<Group> groups = new List<Group>();

                        gc.Groupable = GetAttrValue(xGroups, "groupable", true);

                        foreach (XElement xGroup in xGroups.Elements())
                        {
                            gc.Groups.Add(new Group
                            {
                                Field = GetAttrValue(xGroup, "field", null)
                            });
                        }
                    }

                    XElement xSystemFilter = xDataSource.Element("systemfilter");

                    XElement xGet = xDataSource.Element("get");

                    DataSourceAction actionGet = null;

                    if (xGet != null)
                    {
                        actionGet = new DataSourceAction()
                        {
                            Name = this.GetAttrValue(xGet, "action"),
                            Controller = this.GetAttrValue(xGet, "controller"),
                        };

                        actionGet.Params = new Dictionary<string, string>();

                        foreach (XElement xParam in xGet.Elements())
                        {
                            actionGet.Params.Add(this.GetAttrValue(xParam, "key"), this.GetAttrValue(xParam, "value"));
                        }
                    }

                    DataSource ds = new DataSource()
                    {
                        SystemFilter = GetAttrValue(xSystemFilter, "value", null),
                        Get = actionGet,
                        ServerOperation = GetAttrValue(xDataSource, "serverOperation", "true").ToUpper() == "TRUE",
                        PageSize = Int32.Parse(GetAttrValue(xDataSource, "pageSize", "100")),
                        Sorts = sorts,
                        Filters = filters,
                        Groups = gc,
                        ExecuteQueryBeforeApplyClientFilter = GetAttrValue(xDataSource, "executeQueryBeforeApplyClientFilter", false)
                    };

                    return ds;
                }
            }

            return new DataSource()
            {
                ServerOperation = true,
                PageSize = 100,
                Sorts = new List<Sort>(),
                Filters = new List<Filter>()
            };
        }

        private List<Base.UI.Action> GetHiddenActions(XElement node)
        {
            List<Base.UI.Action> hactions = new List<Base.UI.Action>();

            if (node == null)
            {
                return hactions;
            }

            XElement xHiddenActions = node.Element("hiddenActions");

            if (xHiddenActions != null)
            {
                foreach (XElement xHiddenAction in xHiddenActions.Elements())
                {
                    Base.UI.Action haction = new Base.UI.Action()
                    {
                        ID = xHiddenAction != null ? xHiddenAction.Attribute("id").Value : ""
                    };

                    if (!String.IsNullOrEmpty(haction.ID))
                        hactions.Add(haction);
                }
            }

            return hactions;
        }

        private List<Toolbar> GetToolbars(XElement node)
        {
            List<Toolbar> toolbars = new List<Toolbar>();

            if (node == null)
            {
                return toolbars;
            }

            XElement xToolbars = node.Element("toolbars");
            if (xToolbars != null)
            {
                int i = 0;

                foreach (XElement xToolbar in xToolbars.Elements("toolbar"))
                {
                    XAttribute xToolbarTitle = xToolbar.Attribute("title");
                    XAttribute xToolbarSortOrder = xToolbar.Attribute("sortOrder");
                    XAttribute xToolbarController = xToolbar.Attribute("controller");
                    XAttribute xToolbarAction = xToolbar.Attribute("action");
                    XAttribute xToolbarArea = xToolbar.Attribute("area");
                    XAttribute xToolbarIcon = xToolbar.Attribute("icon");
                    XAttribute xToolbarAjax = xToolbar.Attribute("ajax");
                    XAttribute xIsMaximaze = xToolbar.Attribute("maximize");
                    XAttribute xOnlyForSelected = xToolbar.Attribute("onlyForSelected");

                    if (xToolbarController != null && xToolbarAction != null)
                    {
                        Toolbar toolbar = new Toolbar()
                        {
                            Action = xToolbarAction.Value,
                            Controller = xToolbarController.Value,
                            Area = xToolbarArea != null ? xToolbarArea.Value : null,
                            Title = xToolbarTitle != null ? xToolbarTitle.Value : "",
                            SortOrder = xToolbarSortOrder != null ? Int32.Parse(xToolbarSortOrder.Value) : i,
                            Icon = xToolbarIcon != null ? xToolbarIcon.Value : "",
                            IsAjax = xToolbarAjax != null && (xToolbarAjax.Value.ToUpper() == "TRUE"),
                            IsMaximize = xIsMaximaze != null && (xIsMaximaze.Value.ToUpper() == "TRUE"),
                        };

                        if (xOnlyForSelected != null)
                            toolbar.OnlyForSelected = xOnlyForSelected.Value.ToUpper() == "TRUE";

                        IEnumerable<XElement> parametrs = xToolbar.Elements("param");

                        if (parametrs.Count() != 0)
                        {
                            toolbar.Params = new Dictionary<string, string>();
                        }

                        foreach (XElement param in parametrs)
                        {
                            toolbar.Params.Add(param.Attribute("key") != null ? param.Attribute("key").Value : "",
                                param.Attribute("value") != null ? param.Attribute("value").Value : "");
                        }

                        toolbars.Add(toolbar);
                    }

                    i++;
                }
            }

            return toolbars;
        }

        private List<Step> GetSteps(XElement node, List<Base.UI.Editor> editors)
        {
            var steps = new List<Step>();

            if (node == null)
            {
                return steps;
            }

            var xSteps = node.Element("steps");

            if (xSteps != null)
            {
                var index = 0;

                foreach (var xStep in xSteps.Elements("step"))
                {
                    var xName = xStep.Attribute("name");
                    var xTitle = xStep.Attribute("title");
                    var xDescription = xStep.Element("description");

                    var step = new Step()
                    {
                        Index = index,
                        Name = xName != null ? xName.Value : string.Empty,
                        Title = xTitle != null ? xTitle.Value : string.Empty,
                        Description = xDescription != null ? xDescription.Value : string.Empty,
                        AjaxForm = this.GetAjaxFormAction(xStep),
                    };

                    var xProperties = xStep.Element("properties");

                    if (xProperties != null)
                    {
                        var properties = xProperties.Elements("property").Select(xProperty => new StepProperty()
                        {
                            Name = xProperty.Attribute("name").Value
                        }
                        ).ToList();

                        step.StepProperties = properties;
                    }

                    steps.Add(step);
                    index += 1;
                }
            }

            return steps;
        }

        private List<Column> GetColumns(XElement node)
        {
            List<Column> columns = new List<Column>();

            if (node == null)
            {
                return columns;
            }

            XElement xColumns = node.Element("columns");

            if (xColumns != null)
            {

                foreach (XElement xColumn in xColumns.Elements())
                {
                    XAttribute xPropertyName = xColumn.Attribute("propertyName");
                    XAttribute xMnemonic = xColumn.Attribute("mnemonic");
                    XAttribute xTitle = xColumn.Attribute("title");
                    XAttribute xHidden = xColumn.Attribute("hidden");
                    XAttribute xVisible = xColumn.Attribute("visible");
                    XAttribute xFilterable = xColumn.Attribute("filterable");
                    XAttribute xOrder = xColumn.Attribute("order");
                    XAttribute xLocked = xColumn.Attribute("locked");
                    XAttribute xLockable = xColumn.Attribute("lockable");

                    if (xPropertyName != null)
                    {
                        Column col = new Column(xPropertyName.Value);

                        if (xMnemonic != null)
                            col.Mnemonic = xMnemonic.Value;

                        if (xTitle != null)
                            col.Title = xTitle.Value;

                        if (xHidden != null)
                            col.Hidden = xHidden.Value.ToUpper() == "TRUE";
                        else if (xVisible != null)
                            col.Visible = xVisible.Value.ToUpper() == "TRUE";

                        if (xFilterable != null)
                            col.Filterable = xFilterable.Value.ToUpper() == "TRUE";

                        if (xOrder != null)
                            col.Order = Int32.Parse(xOrder.Value);

                        if (xLocked != null)
                            col.Locked = xLocked.Value.ToUpper() == "TRUE";

                        if (xLockable != null)
                            col.Lockable = xLockable.Value.ToUpper() == "TRUE";

                        columns.Add(col);
                    }

                }
            }

            return columns;
        }

        private List<ConditionalAppearance> GetConditionalAppearance(XElement node)
        {
            List<ConditionalAppearance> conditionalAppearance = new List<ConditionalAppearance>();

            if (node == null)
                return conditionalAppearance;

            XElement xConditionalAppearance = node.Element("condappearance");

            if (xConditionalAppearance != null)
            {
                conditionalAppearance.AddRange(xConditionalAppearance.Elements("add")
                    .Select(x => new
                    {
                        Condition = x.With(c => c.Attribute("condition")).With(c => c.Value),
                        Color = x.With(c => c.Attribute("iconColor")).With(c => c.Value, "black"),
                        Icon = x.With(c => c.Attribute("icon")).With(c => c.Value),
                        Backgound = x.With(c => c.Attribute("bg")).With(c => c.Value, "")
                    })
                    .Where(x => !String.IsNullOrEmpty(x.Condition))
                    .Select(x => new ConditionalAppearance
                    {
                        Color = x.Color,
                        Condition = x.Condition,
                        Icon = x.Icon,
                        Backgound = x.Backgound
                    }));
            }

            return conditionalAppearance;
        }

        private List<Editor> GetEditors(XElement node)
        {
            List<Editor> editors = new List<Editor>();

            if (node == null)
            {
                return editors;
            }

            XElement xEditors = node.Element("editors");

            if (xEditors != null)
            {

                foreach (XElement xEditor in xEditors.Elements())
                {
                    XAttribute xPropertyName = xEditor.Attribute("propertyName");
                    XAttribute xMnemonic = xEditor.Attribute("mnemonic");
                    XAttribute xTitle = xEditor.Attribute("title");
                    XAttribute xIsLabelVisible = xEditor.Attribute("isLabelVisible");
                    XAttribute xEditorTemplate = xEditor.Attribute("editorTemplate");
                    XAttribute xTabName = xEditor.Attribute("tabName");
                    XAttribute xIsReadOnly = xEditor.Attribute("isReadOnly");
                    XAttribute xIsRequired = xEditor.Attribute("isRequired");
                    XAttribute xOrder = xEditor.Attribute("order");
                    XAttribute xVisible = xEditor.Attribute("visible");


                    if (xPropertyName != null)
                    {
                        Editor editor = new Editor(xPropertyName.Value);

                        if (xMnemonic != null)
                            editor.Mnemonic = xMnemonic.Value;

                        if (xTitle != null)
                            editor.Title = xTitle.Value;

                        if (xIsLabelVisible != null)
                            editor.IsLabelVisible = xIsLabelVisible.Value.ToUpper() == "TRUE";

                        if (xEditorTemplate != null)
                            editor.EditorTemplate = xEditorTemplate.Value;

                        if (xTabName != null)
                            editor.TabName = xTabName.Value;

                        if (xIsReadOnly != null)
                            editor.IsReadOnly = xIsReadOnly.Value.ToUpper() == "TRUE";

                        if (xIsRequired != null)
                            editor.IsRequired = xIsRequired.Value.ToUpper() == "TRUE";

                        if (xOrder != null)
                            editor.Order = Int32.Parse(xOrder.Value);

                        if (xVisible != null)
                            editor.Visible = xVisible.Value.ToUpper() == "TRUE";

                        editors.Add(editor);
                    }

                }
            }

            return editors;
        }

        private string GetAttrValue(XElement el, string nameAttr, string def = null)
        {
            if (el != null)
            {
                XAttribute attr = el.Attribute(nameAttr);

                if (attr != null)
                {
                    return attr.Value;
                }
            }

            return def;
        }

        private T GetAttrValue<T>(XElement el, string nameAttr, T def = default (T))
        {
            if (el != null)
            {
                XAttribute attr = el.Attribute(nameAttr);

                if (attr != null)
                {
                    return (T)Convert.ChangeType(attr.Value, typeof(T));
                }
            }

            return def;
        }
    }
}