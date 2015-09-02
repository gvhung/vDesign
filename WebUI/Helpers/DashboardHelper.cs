using Base.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using WebUI.Models.Dashboard.Widgets;

namespace WebUI.Helpers
{
    public static class DashboardHelper
    {
        public static DashboardWidgetBuilder DashboardWidget(this HtmlHelper htmlHelper)
        {
            return new DashboardWidgetBuilder(htmlHelper);
        }

        public static DashboardWidgets Widgets(this HtmlHelper htmlHelper)
        {
            return new DashboardWidgets(htmlHelper);
        }

        public static DashboardWidgetBuilder DashboardWidget(this HtmlHelper htmlhelper, DashboardWidget model)
        {
            return new DashboardWidgetBuilder(htmlhelper, model);
        }
    }

    public class DashboardWidgets
    {
        private readonly HtmlHelper _htmlHelper;

        public DashboardWidgets(HtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public CounterWidgetBuilder CounterWidget(DashboardWidget model, string wrapID = null)
        {
            return new CounterWidgetBuilder(_htmlHelper, model, wrapID ?? "counter_" + Guid.NewGuid().ToString("N"));
        }
    }

    public class CounterWidgetBuilder : IHtmlString
    {
        private readonly HtmlHelper _htmlHelper;
        private readonly string _id;
        private readonly CounterWidget _model;

        internal CounterWidgetBuilder(HtmlHelper htmlHelper, DashboardWidget model, string id)
        {
            _htmlHelper = htmlHelper;
            _id = id;
            _model = new CounterWidget(model)
            {
                WrapID = id
            };
        }

        public CounterWidgetBuilder ItemTemplate(Func<object, object> value)
        {
            var result = value(null);

            if (result != null)
                _model.ItemsTemplate = result.ToString();

            return this;
        }

        public CounterWidgetBuilder AdditionalScript(Func<object, object> value)
        {
            var result = value(_id);

            if (result != null)
                _model.AdditionalScript = result.ToString();

            return this;
        }

        public CounterWidgetBuilder CountUrl(string countUrl)
        {
            _model.CountUrl = countUrl;
            return this;
        }

        public CounterWidgetBuilder ItemsUrl(string itemsUrl)
        {
            _model.ItemsUrl = itemsUrl;
            return this;
        }

        public string ToHtmlString()
        {
            return _htmlHelper.Partial("~/Views/Dashboard/Widgets/_CounterWidget.cshtml", _model).ToHtmlString();
        }

        public CounterWidgetBuilder Items(IEnumerable<CounterMnemonicVm> items)
        {
            _model.Items = items.ToList();

            return this;
        }

        public CounterWidgetBuilder FirstElement(Func<object, HelperResult> firstElement)
        {
            var result = firstElement(_id);

            if (result != null)
                _model.FirstElement = result.ToString();

            return this;
        }

        public CounterWidgetBuilder MaxItemsCount(int i)
        {
            _model.MaxItemsCount = i;

            return this;
        }
    }

    public class DashboardWidgetBuilder : IHtmlString
    {
        private readonly HtmlHelper _htmlHelper;
        private Widget _model;

        public DashboardWidgetBuilder(HtmlHelper htmlHelper)
            :this(htmlHelper, htmlHelper.ViewData.Model as DashboardWidget)
        {

        }

        public DashboardWidgetBuilder(HtmlHelper htmlHelper, DashboardWidget model)
        {
            _htmlHelper = htmlHelper;

            _model = Framework.ObjectHelper.CreateAndCopyObject<Widget>(model);
        }

        public DashboardWidgetBuilder Content(Func<object, object> value)
        {
            var result = value(null);

            if (result != null)
                _model.Html = result.ToString();

            return this;
        }

        public DashboardWidgetBuilder HtmlAttrs(object attrs)
        {
            _model.HtmlAttributes = attrs;

            return this;
        }
 
        public DashboardWidgetBuilder Content(string value)
        {
            _model.Html = value;
            return this;
        }

        public string ToHtmlString()
        {
            return _htmlHelper.Partial("~/Views/Dashboard/Widgets/Base.cshtml", _model).ToHtmlString();
        }

        public class Widget : DashboardWidget
        {
            private string _html;

            public string Html
            {
                get { return _html; }
                set { _html = value; }
            }

            public object HtmlAttributes { get; set; }
        }
    }
}