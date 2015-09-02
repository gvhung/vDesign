using Framework.Maybe;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using WebUI.Controllers;

namespace WebUI.Helpers
{
    public class ContentEditorBuilder : IHtmlString
    {
        private readonly HtmlHelper _htmlHelper;
        private readonly string _name;
        private Type _modelType;
        private string _mnemonic;
        private string _wrap;
        private string _template;
        private string _title;
        private MvcHtmlString _titleTemplate;
        private string _extender;

        public ContentEditorBuilder Mnemonic(string mnemonic)
        {
            var controller = _htmlHelper.ViewContext.Controller as IBaseController;
            if (controller != null)
            {
                var config = controller.GetViewModelConfig(mnemonic);

                if (config != null)
                    _modelType = config.TypeEntity;
            }

            _mnemonic = mnemonic;
            return this;
        }

        public ContentEditorBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentEditorBuilder Template(string templateID)
        {
            _template = templateID;
            return this;
        }

        public ContentEditorBuilder ExtendWith(Func<object, HelperResult> extender)
        {
            var str = extender(null).ToHtmlString();

            if (string.IsNullOrEmpty(str))
            {
                _extender = "{}";
            }
            else
            {
                _extender = str.Replace("<script>", "").Replace("</script>", "");
            }

            return this;
        }

        public ContentEditorBuilder(string name, HtmlHelper htmlHelper)
        {
            _name = name;
            _htmlHelper = htmlHelper;
        }

        public ContentEditorBuilder TitleTemplate(Func<object, HelperResult> template)
        {
            _titleTemplate = MvcHtmlString.Create(template(null).ToString());
            return this;
        }

        public ContentEditorBuilder Wrap(string wrap)
        {
            _wrap = wrap;
            return this;
        }

        public string ToHtmlString()
        {
            return _htmlHelper.Partial("~/Views/Shared/ContentWidgets/BaseContentWidget.cshtml", new ContentEditorWidget(this)).ToHtmlString();
        }

        public class ContentEditorWidget
        {
            private readonly ContentEditorBuilder _builder;
            private object _model;

            public ContentEditorWidget(ContentEditorBuilder builder)
            {
                _builder = builder;
            }

            public string Name
            {
                get { return _builder._name; }
            }

            public string Mnemonic
            {
                get { return _builder._mnemonic; }
            }

            public MvcHtmlString TitleTemplate
            {
                get { return _builder._titleTemplate; }
            }

            public string Template
            {
                get { return _builder._template; }
            }

            public string Wrap
            {
                get { return _builder._wrap; }
            }

            public string Extender
            {
                get { return string.IsNullOrEmpty(_builder._extender) ? "{}" : _builder._extender; }
            }

            public object Model
            {
                get { return _model ?? (_model = _builder._modelType.With(Activator.CreateInstance)); }
            }

            public string Title
            {
                get { return _builder._title; }
            }
        }
    }

    public static class ContentHelper
    {
        public static ContentEditorBuilder ContentEditorWidget(this HtmlHelper htmlHelper, string name)
        {
            return new ContentEditorBuilder(name, htmlHelper);
        }
    }
}