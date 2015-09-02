using Base;
using Base.Attributes;
using Base.Entities.Complex;
using Base.EntityFrameworkTypes.Complex;
using Base.Security;
using Base.UI;
using Base.UI.Presets;
using Framework;
using Kendo.Mvc;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using WebUI.Models;

namespace WebUI.Helpers
{
    public class ColumnsInitializationResult
    {
        private readonly Dictionary<string, object> _propertyTypes = new Dictionary<string, object>();

        public Dictionary<string, object> PropertyTypes
        {
            get { return _propertyTypes; }
        }

        public StandartGridView GridModel { get; set; }

        public ColumnsInitializationResult(StandartGridView gridModel)
        {
            GridModel = gridModel;
        }

        public void AddColumnInfo(ColumnSetting column, object o)
        {
            _propertyTypes.Add(column.Name, o);
        }
        
        public void AddColumnInfo(string column, object o)
        {
            _propertyTypes.Add(column, o);
        }

    }

    public static class KendoGridHelper
    {
        public static void InitColumns<TModel>(this GridColumnFactory<TModel> factory, WebViewPage<StandartGridView> webViewPage,
            Action<ColumnSetting, GridColumnFactory<TModel>> callback = null) where TModel : class
        {
            var preset = webViewPage.Model.Preset;

            if (webViewPage.Model.ViewModelConfig.ListView.ConditionalAppearance.Count > 0)
            {
                bool frozenColumns = preset.Columns.Any(x => x.ColumnViewModel.Locked);

                factory.InitConditionalAppearanceColumn(webViewPage.Model.ViewModelConfig.ListView.ConditionalAppearance, frozenColumns);
            }

            var initializationResult = new ColumnsInitializationResult(webViewPage.Model);

            foreach (ColumnSetting column in preset.Columns.Where(c => c.Visible).OrderBy(x => x.SortOrder))
            {
                ColumnViewModel columnViewModel = column.ColumnViewModel;

                PropertyDataTypeAttribute datatype = columnViewModel.PropertyInfo.GetCustomAttribute<PropertyDataTypeAttribute>();

                if (columnViewModel.PropertyType.IsBaseObject())
                {
                    if (typeof(FileData).IsAssignableFrom(columnViewModel.PropertyType))
                    {
                        if (datatype != null && datatype.CustomDataType == "Image")
                        {
                            factory.InitImageColumn(column);
                        }
                        else
                        {
                            factory.InitFileColumn(column);
                        }
                    }
                    else if (typeof(BaseUser).IsAssignableFrom(columnViewModel.PropertyType))
                    {
                        factory.InitUserColumn(column, webViewPage.Model);
                    }
                    else
                    {
                        factory.InitBaseObjectColumn(column, webViewPage.Model);
                    }
                }
                else if (columnViewModel.PropertyType.IsBaseCollection())
                {
                    factory.InitCollectionBaseObjectColumn(column, webViewPage.Model);

                    // ??
                    string colName;

                    if (column.ColumnViewModel.ColumnType.IsAssignableToGenericType(typeof(EasyCollectionEntry<>)))
                        colName = column.Name + JsonNetResult.EASY_COLLECTION_SUFFIX;
                    else
                        colName = column.Name + JsonNetResult.BASE_COLLECTION_SUFFIX;

                    initializationResult.AddColumnInfo(colName, new 
                    { 
                        columnType = "CollectionBaseObject",
                        realName = column.Name,
                        lookup = column.ColumnViewModel.ViewModelConfig.LookupProperty
                    });
                }
                else
                {
                    if (column.Type.IsEnum())
                    {
                        factory.InitEnumColumn(column, webViewPage.Model);

                        initializationResult.AddColumnInfo(column, new { columnType = "enum", propertyType = column.ColumnViewModel.PropertyType.FullName });
                    }
                    else if (column.Type == typeof(string))
                    {
                        bool addStringColumn = false;

                        if (datatype != null)
                        {
                            if (datatype.CustomDataType == "ListBaseObjects" || datatype.CustomDataType == "ListWFObjects")
                            {
                                addStringColumn = true;

                                factory.InitListBaseObjectsColumn(column);

                            }
                            else if (datatype.CustomDataType == "Color")
                            {
                                addStringColumn = true;

                                factory.InitColorColumn(column);
                            }
                            else if (datatype.DataType == PropertyDataType.Url)
                            {
                                addStringColumn = true;

                                factory.InitStringColumn(column, webViewPage.Model, String.Format("<a class='cell-link' href='#= data.{0} #'>#= pbaAPI.truncateStr(data.{0}, 150) #</a>", column.Name));
                            }
                        }

                        if (!addStringColumn)
                        {
                            factory.InitStringColumn(column, webViewPage.Model);
                        }
                    }
                    else if (column.Type == typeof(bool))
                    {
                        factory.InitBoolColumn(column);
                    }
                    else if (column.Type == typeof(Period))
                    {
                        factory.InitPeriodColumn(column, webViewPage.Model);
                    }
                    else if (typeof(MultilanguageText).IsAssignableFrom(column.Type))
                    {
                        factory.InitMultilanguageColumn(column);
                    }
                    else if (typeof(Statistic).IsAssignableFrom(column.Type))
                    {
                        factory.InitStatisticColumn(column);

                        initializationResult.AddColumnInfo(column, new { columnType = "statistic" });
                    }
                    else if (typeof(Location).IsAssignableFrom(column.Type))
                    {
                        factory.InitLocationColumn(column);
                    }
                    else if (column.Type == typeof(DateTime) || column.Type == typeof(DateTime?))
                    {
                        factory.InitDateTimeColumn(column, datatype);
                    }
                    else if (column.Type == typeof(decimal) || column.Type == typeof(decimal?))
                    {
                        factory.InitDecimalColumn(column);
                    }
                    else if (column.Type == typeof(double) || column.Type == typeof(double?))
                    {
                        if (datatype != null)
                        {
                            if (datatype.CustomDataType == "Percent")
                                factory.InitPercentColumn(column);
                        }
                        else
                        {
                            factory.InitDoubleColumn(column);
                        }
                    }
                    else if (column.Type == typeof(LinkBaseObject))
                    {
                        factory.InitLinkBaseObjectColumn(column);
                    }
                    else if (column.Type == typeof(Color))
                    {
                        factory.InitColorColumn(column);
                    }
                    else
                    {
                        if (columnViewModel.ViewModelConfig != null)
                        {
                            string lookupPropertyForUI = columnViewModel.ViewModelConfig.LookupPropertyForUI;

                            factory.Bound(typeof(string), column.Name + "." + lookupPropertyForUI)
                                .Title(column.Title)
                                .Width(column.Width ?? 200)
                                .Sortable(columnViewModel.Sortable)
                                .Filterable(columnViewModel.Filterable);
                        }
                        else
                        {
                            factory.Bound(column.Type, column.Name)
                                .Title(column.Title)
                                .Width(column.Width ?? 200)
                                .Sortable(columnViewModel.Sortable)
                                .Filterable(columnViewModel.Filterable);
                        }

                    }
                }

                if (callback != null)
                {
                    callback(column, factory);
                }
            }

            webViewPage.Html.RenderPartial("~/Views/Standart/_ColumnsInitialization.cshtml", initializationResult);
        }

        public static GridBoundColumnBuilder<T> InitColumn<T>(this GridBoundColumnBuilder<T> columnBuilder, ColumnSetting column, int? width = null) where T : class
        {
            return columnBuilder
                .Title(column.Title)
                .Sortable(column.ColumnViewModel.Sortable)
                .Filterable(column.ColumnViewModel.Filterable)
                .Locked(column.ColumnViewModel.Locked)
                .Lockable(column.ColumnViewModel.Lockable)
                .Groupable(column.ColumnViewModel.Groupable)
                .Width(column.Width ?? width ?? 200)
                .Hidden(!column.Visible);
        }

        public static GridColumnFactory<TModel> InitStringColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, StandartGridView gridView, string clientTemplate = null) where TModel : class
        {
            int width = 300;

            if (column.ColumnViewModel.MaxLength != null)
            {
                if (column.ColumnViewModel.MaxLength <= 100)
                    width = 100;
                else if (column.ColumnViewModel.MaxLength <= 255)
                    width = 250;
            }

            factory.Bound(column.Type, column.Name)
                .InitColumn(column, width)
                .ClientTemplate(clientTemplate ?? String.Format("#= pbaAPI.truncateStr(pbaAPI.htmlEncode(data.{0}), 300) #", column.Name))
                .Filterable(f =>
                {
                    if (column.ColumnViewModel.Filterable)
                    {
                        f.UI("function(element){ pbaAPI.gridStringColumnFilterUi(window['" + gridView.WidgetID + "'], '" + column.Name + "', element); }");
                        f.Operators(op => op.ForString(x => x.Clear().Contains("Содержит").DoesNotContain("Не содержит").IsEqualTo("Равно").IsNotEqualTo("Не равно")));
                    }
                    else
                    {
                        f.Enabled(false);
                    }
                });

            return factory;
        }

        public static GridColumnFactory<TModel> InitBaseObjectColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, StandartGridView gridView) where TModel : class
        {
            factory.InitBaseObjectColumn(column, gridView,
                String.Format("#= pbaAPI.getPrVal(data, '{0}.{1}', '') #", column.Name, column.ColumnViewModel.ViewModelConfig.LookupPropertyForUI));

            return factory;
        }

        public static GridColumnFactory<TModel> InitBaseObjectColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, StandartGridView gridView, string template) where TModel : class
        {
            ViewModelConfig config = column.ColumnViewModel.ViewModelConfig;

            string lookupPropertyForUI = config.LookupPropertyForUI;
            string lookupPropertyForFilter = config.LookupPropertyForFilter;

            Type typeLookupProperty = config.TypeEntity.GetProperty(config.LookupProperty).PropertyType;

            bool isMultilanguageText = typeof(Base.Entities.Complex.MultilanguageText).IsAssignableFrom(typeLookupProperty);

            factory.Bound(typeof(int), column.Name + "." + lookupPropertyForUI)
                .InitColumn(column, 300)
                .ClientTemplate(template)
                .Filterable(f =>
                {
                    if (column.ColumnViewModel.Filterable)
                    {
                        f.UI("function(element){ pbaAPI.gridBaseObjectColumnFilterUi(" +
                            "{" +
                                "grid: window['" + gridView.WidgetID + "']," +
                                "mnemonic: '" + column.Type.FullName + "'," +
                                "colName: '" + column.Name + "'," +
                                "lookuppropery: '" + lookupPropertyForFilter + "'," +
                                "element: element," +
                            "})}");
                        f.Operators(op => op.ForString(x => x.Clear().IsEqualTo("Равно").IsNotEqualTo("Не равно")));
                        f.Extra(false);
                    }
                    else
                    {
                        f.Enabled(false);
                    }
                });

            return factory;
        }

        public static GridColumnFactory<TModel> InitCollectionBaseObjectColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, StandartGridView gridView) where TModel : class
        {
            ViewModelConfig config = column.ColumnViewModel.ViewModelConfig;

            string lookupProperty = config.LookupProperty;
            string lookupPropertyForUI = config.LookupPropertyForUI;
            string lookupPropertyForFilter = config.LookupPropertyForFilter;

            string colName;

            if (column.ColumnViewModel.ColumnType.IsAssignableToGenericType(typeof(EasyCollectionEntry<>)))
                colName = column.Name + JsonNetResult.EASY_COLLECTION_SUFFIX;
            else
                colName = column.Name + JsonNetResult.BASE_COLLECTION_SUFFIX;
            

            Type typeLookupProperty = config.TypeEntity.GetProperty(config.LookupProperty).PropertyType;

            bool isMultilanguageText = typeof(Base.Entities.Complex.MultilanguageText).IsAssignableFrom(typeLookupProperty);

            factory.Bound(typeof(string), colName)
                .InitColumn(column, 300)
                .ClientTemplate(String.Format("#= pbaAPI.getCollectionPrVal(data.{0}, '{1}', '') #", column.Name, lookupPropertyForUI))
                .Filterable(f =>
                {
                    if (column.ColumnViewModel.Filterable)
                    {
                        f.UI("function(element){ pbaAPI.gridBaseObjectColumnFilterUi(" +
                            "{" +
                                "grid: window['" + gridView.WidgetID + "']," +
                                "mnemonic: '" + column.Type.FullName + "'," +
                                "colName: '" + colName + "'," +
                                "lookuppropery: '" + lookupPropertyForFilter + "'," +
                                "element: element," +
                                "isBasecollection: true" +
                            "})}");
                        f.Operators(op => op.ForString(x => x.Clear().IsEqualTo("Равно").IsNotEqualTo("Не равно")));
                        f.Extra(false);
                    }
                    else
                    {
                        f.Enabled(false);
                    }
                })
                .Sortable(false)
                .Groupable(false);

            return factory;
        }

        public static GridColumnFactory<TModel> InitEnumColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, StandartGridView gridView) where TModel : class
        {
            Type enumType;

            if (column.Type.IsEnum)
                enumType = column.Type;
            else
                enumType = column.Type.GetGenericArguments()[0];

            string values = Newtonsoft.Json.JsonConvert.SerializeObject(Enum.GetValues(enumType).Cast<Enum>()
               .Select(v => new
               {
                   Value = v.GetValue(),
                   Name = v.ToString(),
                   Text = v.GetDescription()
               }));

            factory.Bound(typeof(int), column.Name)
                .InitColumn(column, 220)
                .ClientTemplate(
                    String.Format("# application.initEnumValues('{0}','{1}') #", enumType.FullName, values) +
                    String.Format("<span class='enum-{0}' data-val='#=application.getEnumName('{1}', data.{2})#'>#= application.getEnumText('{1}', data.{2}) #<span>", enumType.Name, enumType.FullName, column.Name))
                .Filterable(f =>
                {
                    if (column.ColumnViewModel.Filterable)
                    {
                        f.UI("function(element){ pbaAPI.gridEnumColumnFilterUi(window['" + gridView.WidgetID + "'], '" + column.Name + "', '" + enumType.Name + "','" + values + "', element); }");
                        f.Extra(false);
                    }
                    else
                    {
                        f.Enabled(false);
                    }
                })
                .ClientGroupHeaderTemplate(String.Format("<span class='enum-{0}' data-val='#=application.getEnumName('{1}', value)#'>#= application.getEnumText('{1}', value) #<span>", enumType.Name, enumType.FullName, column.Name));

            return factory;
        }

        public static GridColumnFactory<TModel> InitListBaseObjectsColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(typeof(int), column.Name)
                .InitColumn(column, 200)
                .ClientTemplate(String.Format("#= application.viewModelConfigs.getConfig(data.{0}).Title #", column.Name))
                .Filterable(f =>
                {
                    if (column.ColumnViewModel.Filterable)
                    {
                        f.UI("function(element){ pbaAPI.gridColumnFilterUi(application.viewModelConfigs.getTypes(), element); }");
                        f.Operators(op => op.ForString(x => x.Clear().IsEqualTo("Равно").IsNotEqualTo("Не равно")));
                    }
                    else
                    {
                        f.Enabled(false);
                    }
                });

            return factory;
        }

        public static GridColumnFactory<TModel> InitLinkBaseObjectColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(typeof(string), String.Format("{0}.FullName", column.Name))
                .InitColumn(column, 200)
                .ClientTemplate("# if (data.{0}.FullName) { # <span class='#= application.viewModelConfigs.getConfig(data.{0}.Mnemonic || data.{0}.FullName).Icon #'>&nbsp;</span><a class='cell-link' href='javascript: void(0)' onclick=\"pbaAPI.openViewModelEx('#= {0}.Mnemonic || {0}.FullName #', { wid: '{1}', title: 'Объект', isMaximaze: true, id: #= {0}.ID #});\"> #= application.viewModelConfigs.getConfig(data.{0}.Mnemonic || data.{0}.FullName).Title #</a> # } #".Replace("{0}", column.Name).Replace("{1}", Guid.NewGuid().ToString("N")))
                .Filterable(f =>
                {
                    if (column.ColumnViewModel.Filterable)
                    {
                        f.UI("function(element){ pbaAPI.gridColumnFilterUi(application.viewModelConfigs.getTypes(), element); }");
                        f.Operators(op => op.ForString(x => x.Clear().IsEqualTo("Равно").IsNotEqualTo("Не равно")));
                    }
                    else
                    {
                        f.Enabled(false);
                    }
                });

            return factory;
        }

        public static GridColumnFactory<TModel> InitImageColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            int width = column.Width ?? 100;
            int height = column.ColumnViewModel.Height ?? 100;

            factory.Bound(column.Type, column.Name)
                .InitColumn(column, width + 15)
                .ClientTemplate(String.Format(
                    "<img src='#= {0} != null ? pbaAPI.imageHelpers.getsrc({0}.FileID, {1}, {2}) : pbaAPI.imageHelpers.getsrc(null, {1}, {2}) #'>", column.Name, width, height))
                .HtmlAttributes(new { style = "text-align: center;" })
                .HeaderHtmlAttributes(new { width = width + 15 })
                .Filterable(false)
                .Sortable(false);

            return factory;
        }

        public static GridColumnFactory<TModel> InitUserColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, StandartGridView gridView) where TModel : class
        {
            factory.InitBaseObjectColumn(column, gridView,
                String.Format("#= pbaAPI.getUserStr({0}) #", column.Name));

            return factory;
        }

        public static GridColumnFactory<TModel> InitFileColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(column.Type, column.Name)
                .InitColumn(column, 75)
                .ClientTemplate(string.Format(@"#= pbaAPI.getFilePreviewHtml({0}) #", column.Name))
                .Filterable(false)
                .Sortable(false);

            return factory;
        }

        public static GridColumnFactory<TModel> InitColorColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            string propertyName = column.Name;

            if (column.Type == typeof(Color))
            {
                propertyName += ".Value";
            }

            factory.Bound(column.Type, column.Name)
                .InitColumn(column, 150)
                .ClientTemplate(String.Format("<span data-bg='#= {0} #' class='m-icon'></span>", propertyName))
                .Filterable(false)
                .Sortable(false);

            return factory;
        }

        public static GridColumnFactory<TModel> InitBoolColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(column.Type, column.Name)
                .InitColumn(column, 100)
                .ClientTemplate(String.Format("<span class='k-icon #= {0} ? 'icon-yes' : 'icon-no' #'></span>", column.Name))
                .HtmlAttributes(new { @class = "cell-bool" })
                .Filterable(false)
                .Sortable(false);

            return factory;
        }

        public static GridColumnFactory<TModel> InitStatisticColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            string property = column.Name;

            factory.Bound(typeof(string), property)
                .InitColumn(column, 150)
                .ClientTemplate(
                    String.Format(
                    "<div class='stats-wrap' style='min-width: 120px;'>" +
                        "<span><i data-views class='glyphicon glyphicon-eye-open'></i><span>#= {0}.Views #</span></span>" +
                        "<span><i data-rate class='halfling halfling-star'></i><span>#= {0}.Rating #</span></span>" +
                        "<span><i data-comment class='glyphicon glyphicon-quote'></i><span>#= {0}.Comments #</span></span>" +
                    "</div>",
                    property)
                 )
                .Title(" ")
                .Sortable(false)
                .Filterable(false);

            return factory;
        }

        public static GridColumnFactory<TModel> InitPeriodColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, StandartGridView gridView) where TModel : class
        {
            string property = column.Name + ".Start";

            factory.Bound(typeof(DateTime), property)
                .InitColumn(column, 220)
                .ClientTemplate("<span class='glyphicon glyphicon-calendar'>&nbsp;</span>" + String.Format("#= {0}.Start != null ? kendo.toString(kendo.parseDate({0}.Start, '{1}'), '{2}') : '' #", column.Name, JsonNetResult.DATE_TIME_FORMATE, JsonNetResult.DATE_FORMATE) +
                " ~ " + String.Format("#= {0}.End != null ? kendo.toString(kendo.parseDate({0}.End, '{1}'), '{2}') : '' #", column.Name, JsonNetResult.DATE_TIME_FORMATE, JsonNetResult.DATE_FORMATE))
                .Filterable(f =>
                {
                    if (column.ColumnViewModel.Filterable)
                    {
                        f.UI("function(element){ pbaAPI.gridPeriodColumnFilterUi(window['" + gridView.WidgetID + "'], '" + property + "','" + JsonNetResult.DATE_TIME_FORMATE + "', element); }");
                        f.Extra(false);
                    }
                    else
                    {
                        f.Enabled(false);
                    }
                });

            return factory;
        }

        public static GridColumnFactory<TModel> InitLocationColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(column.Type, column.Name)
                .InitColumn(column, 300)
                .ClientTemplate(String.Format("<span class='halfling halfling-map-marker'>&nbsp;</span>#= ({0} && {0}.Address && {0}.Address.Lang.ru) ? {0}.Address.Lang.ru : 'Отсутсвует' #", column.Name))
                .Filterable(false);

            return factory;
        }

        public static GridColumnFactory<TModel> InitDateTimeColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column, PropertyDataTypeAttribute datatype) where TModel : class
        {
            string formate = JsonNetResult.DATE_FORMATE;
            int width = 150;

            if (datatype != null)
            {
                switch (datatype.DataType)
                {
                    case PropertyDataType.DateTime:
                        formate = JsonNetResult.DATE_TIME_FORMATE;
                        width = 180;
                        break;
                    case PropertyDataType.Month:
                        formate = JsonNetResult.MONTH_FORMATE;
                        break;
                }
            }

            factory.Bound(column.Type, column.Name)
                .InitColumn(column, width)
                .ClientTemplate(String.Format("#= data.{0} != null ? kendo.toString(kendo.parseDate(data.{0}, '{1}'), '{2}') : '' #", column.Name, JsonNetResult.DATE_TIME_FORMATE, formate))
                .HtmlAttributes(new { @class = "cell-date" });

            return factory;
        }

        public static GridColumnFactory<TModel> InitDecimalColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(column.Type, column.Name)
                .InitColumn(column, 150)
                .ClientTemplate(String.Format("#= data.{0} != null ? kendo.toString(data.{0}, 'c') : '' #", column.Name))
                .HtmlAttributes(new { @class = "cell-decimal" });

            return factory;
        }

        public static GridColumnFactory<TModel> InitDoubleColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(column.Type, column.Name)
                .InitColumn(column, 150)
                .ClientTemplate(String.Format("#= data.{0} != null ? kendo.toString(data.{0}, 'n') : '' #", column.Name))
                .HtmlAttributes(new { @class = "cell-double" });

            return factory;
        }

        public static GridColumnFactory<TModel> InitPercentColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(column.Type, column.Name)
                .InitColumn(column, 150)
                .ClientTemplate(String.Format("<div class='progress'><div class='progress-bar' role='progressbar' aria-valuenow='#= data.{0} != null ? kendo.toString(data.{0} * 100, 'n0') : '0' #' aria-valuemin='0' aria-valuemax='100' style='width: #= data.{0} != null ? kendo.toString(data.{0} * 100, 'n0') : '0' #%;'>#= data.{0} != null ? kendo.toString(data.{0} * 100, 'n0') : '0' #%</div></div>", column.Name))
                .HtmlAttributes(new { @class = "cell-percent" });

            //"<div class='progress'><div class='progress-bar' role='progressbar' aria-valuenow='#= data.{0} != null ? kendo.toString(data.{0}, 'n') : '0' #' aria-valuemin='0' aria-valuemax='100' style='width: #= data.{0} != null ? kendo.toString(data.{0}, 'n') : '0' #%;'>#= data.{0} != null ? kendo.toString(data.{0}, 'n') : '0' #%</div></div>"
            return factory;
        }

        public static GridColumnFactory<TModel> InitMultilanguageColumn<TModel>(this GridColumnFactory<TModel> factory, ColumnSetting column) where TModel : class
        {
            factory.Bound(typeof(string), column.Name + ".Xml")
                .InitColumn(column, 300)
                .ClientTemplate(String.Format("#= pbaAPI.truncateStr(data.{0}.Lang['ru'], 150) #", column.Name))
                .Filterable(f => f
                .Operators(op => op.ForString(x => x.Clear().Contains("Содержит").DoesNotContain("Не содержит"))));

            return factory;
        }

        public static GridColumnFactory<TModel> InitConditionalAppearanceColumn<TModel>(this GridColumnFactory<TModel> factory, List<ConditionalAppearance> conds, bool frozenColumns) where TModel : class
        {
            StringBuilder sb = new StringBuilder("#if(false){}");

            foreach (ConditionalAppearance cond in conds)
                sb.AppendFormat("else if(data.{0}){{# <span data-bg='{3}' class='m-icon {1}' style='color:{2};'></span> # }}", cond.Condition, cond.Icon, cond.Color, cond.Backgound);

            sb.Append("#");

            var builder = factory.Bound(typeof(string), "")
                .HtmlAttributes(new { style = "text-align: center;" })
                .Title("")
                .Filterable(false)
                .Sortable(false)
                .ClientTemplate(sb.ToString())
                .Width(10);

            if (frozenColumns)
                builder.Locked(true).Lockable(false);

            return factory;
        }

        public static DataSourceModelDescriptorFactory<T> InitModel<T>(this DataSourceModelDescriptorFactory<T> dataSourceModelDescriptorFactory, WebViewPage<StandartGridView> webViewPage) where T : class
        {
            Type type = webViewPage.Model.ViewModelConfig.TypeEntity;

            List<PropertyInfo> props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            dataSourceModelDescriptorFactory.Id("ID");

            var preset = webViewPage.Model.Preset;

            foreach (ColumnSetting columnSetting in preset.Columns.Where(c => c.Visible))
            {
                PropertyInfo prop = props.FirstOrDefault(x => x.Name == columnSetting.Name);

                dataSourceModelDescriptorFactory.Field(prop.Name, prop.PropertyType);
            }

            return dataSourceModelDescriptorFactory;
        }

        public static DataSourceModelDescriptorFactory<T> InitModelAllProperties<T>(this DataSourceModelDescriptorFactory<T> dataSourceModelDescriptorFactory, WebViewPage<StandartGridView> webViewPage) where T : class
        {
            Type type = webViewPage.Model.ViewModelConfig.TypeEntity;

            dataSourceModelDescriptorFactory.Id("ID");

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType.IsValueType)
                {
                    dataSourceModelDescriptorFactory.Field(prop.Name, prop.PropertyType).DefaultValue(Activator.CreateInstance(prop.PropertyType));
                }
                else
                {
                    dataSourceModelDescriptorFactory.Field(prop.Name, prop.PropertyType);
                }

            }

            return dataSourceModelDescriptorFactory;
        }

        public static DataSourceSortDescriptorFactory<T> InitDataSourceSort<T>(this DataSourceSortDescriptorFactory<T> factory, WebViewPage<StandartGridView> webViewPage) where T : class
        {
            if (webViewPage.Model.ViewModelConfig.ListView.DataSource.Sorts.Count == 0)
            {
                factory.Add("SortOrder").Ascending();
            }
            else
            {
                foreach (Base.UI.Sort s in webViewPage.Model.ViewModelConfig.ListView.DataSource.Sorts.Where(m => !String.IsNullOrEmpty(m.Descriptor)).ToList())
                {
                    if (s.Order == "asc")
                    {
                        factory.Add(s.Descriptor).Ascending();
                    }
                    else
                    {
                        factory.Add(s.Descriptor).Descending();
                    }
                }
            }

            return factory;
        }

        public static DataSourceFilterDescriptorFactory<T> InitDataSourceFilter<T>(this DataSourceFilterDescriptorFactory<T> factory, WebViewPage<StandartGridView> webViewPage) where T : class
        {
            List<Base.UI.Filter> filters = webViewPage.Model.ViewModelConfig.ListView.DataSource.Filters.Where(m => !String.IsNullOrEmpty(m.Field)).ToList();

            if (webViewPage.Model.Filters != null)
                filters.Concat(webViewPage.Model.Filters);

            IList<IFilterDescriptor> descriptors = new List<IFilterDescriptor>();

            if (webViewPage.Model.Type == TypeDialog.Lookup)
            {
                CompositeFilterDescriptor composite = new CompositeFilterDescriptor();

                FilterDescriptor descriptor = new FilterDescriptor("Hidden", FilterOperator.IsEqualTo, false);

                composite.FilterDescriptors.Add(descriptor);

                descriptors.Add(composite);
            }

            foreach (int group in filters.GroupBy(m => m.Group).Select(m => m.Key).OrderBy(m => m))
            {
                CompositeFilterDescriptor composite = new CompositeFilterDescriptor();

                composite.LogicalOperator = FilterCompositionLogicalOperator.Or;

                foreach (Base.UI.Filter f in filters.Where(m => m.Group == group))
                {
                    FilterOperator op = FilterOperator.IsEqualTo;

                    object value;
                    string str = f.Value.ToString();

                    if (str.IndexOf("@CurrentDate", StringComparison.OrdinalIgnoreCase) >= 0)
                        value = DateTime.Today;
                    else if (str.IndexOf("@NextWeek", StringComparison.OrdinalIgnoreCase) >= 0)
                        value = DateTime.Today.AddDays(7);
                    else if (str.IndexOf("@AfterNextWeek", StringComparison.OrdinalIgnoreCase) >= 0)
                        value = DateTime.Today.AddDays(14);
                    else if (str.IndexOf("@CurrentUserID", StringComparison.OrdinalIgnoreCase) >= 0)
                        value = webViewPage.Model.SecurityUser.ID;
                    else if (str.IndexOf("@Today", StringComparison.OrdinalIgnoreCase) >= 0)
                        value = DateTime.Today;
                    else
                        value = str;

                    switch (f.Operator)
                    {
                        case "Contains":
                            op = FilterOperator.Contains;
                            break;

                        case "IsEqualTo":
                        case "eq":
                            op = FilterOperator.IsEqualTo;
                            break;

                        case "IsNotEqualTo":
                        case "neq":
                            op = FilterOperator.IsNotEqualTo;
                            break;

                        case "IsGreaterThanOrEqualTo":
                        case "gte":
                            op = FilterOperator.IsGreaterThanOrEqualTo;
                            break;

                        case "IsLessThanOrEqualTo":
                        case "lte":
                            op = FilterOperator.IsLessThanOrEqualTo;
                            break;
                    }

                    FilterDescriptor descriptor = new FilterDescriptor(f.Field, op, value);

                    composite.FilterDescriptors.Add(descriptor);
                }

                if (composite != null)
                    descriptors.Add(composite);
            }

            if (descriptors.Count > 0)
            {
                factory.AddRange(descriptors);
            }

            return factory;
        }

        public static DataSourceGroupDescriptorFactory<T> InitDataSourceGroup<T>(this DataSourceGroupDescriptorFactory<T> factory, WebViewPage<StandartGridView> webViewPage) where T : class
        {
            //GridPreset preset = webViewPage.Model.GetUserGridPreset(webViewPage.Session);

            //ViewModelConfig config = webViewPage.Model.ViewModelConfig;

            //IList<GroupDescriptor> descriptors = factory.GetType().GetField("descriptors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(factory) as IList<GroupDescriptor>;

            //foreach (Group group in config.ListView.DataSource.Groups)
            //{
            //    ColumnViewModel column = preset[group.Field].ColumnViewModel;

            //    GroupDescriptor groupDescriptor = new GroupDescriptor();

            //    if (column.ColumnType.IsBaseObject())
            //    {
            //        groupDescriptor.Member = column.PropertyName + ".ID";

            //        groupDescriptor.DisplayContent = "PublicationCategory.Title.Xml";

            //        groupDescriptor.MemberType = column.ColumnType;

            //        descriptors.Add(groupDescriptor);
            //    }
            //    else
            //    {
            //        groupDescriptor.Member = column.PropertyName;
            //    }
            //}

            return factory;
        }

        public static void InitEvents(this GridEventBuilder factory, WebViewPage<StandartGridView> webViewPage)
        {
            StandartGridView model = webViewPage.Model;

            factory.DataBound(model.WidgetID + ".onDataBound");
            factory.DataBinding(model.WidgetID + ".onDataBinding");
            factory.Change(model.WidgetID + ".onChange");
            factory.ColumnReorder(model.WidgetID + ".onColumnReorder");
            factory.ColumnResize(model.WidgetID + ".onColumnResize");

            if (model.Type == TypeDialog.Lookup)
            {
                factory.Edit(model.WidgetID + ".onEdit");
                factory.Save(model.WidgetID + ".onSave");
                factory.Cancel(model.WidgetID + ".onCancel");
            }
        }

        public static void InitPageable(this PageableBuilder p, WebViewPage<StandartGridView> webViewPage)
        {
            StandartGridView model = webViewPage.Model;

            if (model.Type == TypeDialog.Lookup)
            {
                p.Enabled(false);
            }
            else
            {
                p.Refresh(true).ButtonCount(5)
                    .PageSizes(false)
                    .Messages(m => m.First("На первую")
                    .Last("На последнию")
                    .Previous("На предыдущую")
                    .Next("На следующую")
                    .Refresh("Обновить")
                    .ItemsPerPage(""));
            }
        }

        public static void InitFilterable(this GridFilterableSettingsBuilder f, WebViewPage<StandartGridView> webViewPage)
        {
            StandartGridView model = webViewPage.Model;
        }

        public static DataSourceAggregateDescriptorFactory<T> InitDataSourceAggregate<T>(this DataSourceAggregateDescriptorFactory<T> factory, WebViewPage<StandartGridView> webViewPage) where T : class
        {
            //if (webViewPage.Model.ViewModelConfig.ListView.DataSource.Groups.Groupable)
            //{
            //    GridPreset preset = webViewPage.Model.GetUserGridPreset(webViewPage.Session);

            //    foreach (ColumnSetting column in preset.Columns.Where(c => c.Visible).OrderBy(x => x.SortOrder))
            //    {
            //        ColumnViewModel columnViewModel = column.ColumnViewModel;

            //        factory.Add(column.Name, columnViewModel.PropertyType).Count();
            //    }
            //}

            return factory;
        }
    }
}