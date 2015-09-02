using Base;
using Base.Entities.Complex;
using Base.UI;
using Framework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebUI.Controllers;

namespace WebUI.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlString RawBo(this HtmlHelper htmlHelper, BaseObject bo, string mnemonic)
        {
            return
                htmlHelper.Raw(new JsonNetResult(bo,
                    (htmlHelper.ViewContext.Controller as IBaseController).GetBoContractResolver(mnemonic)));
        }
        
        public static CommonEditorViewModel GetCommonEditor(this HtmlHelper htmlHelper, string mnemonic)
        {
            var baseController = htmlHelper.ViewContext.Controller as IBaseController;

            return baseController != null ? baseController.GetCommonEditor(mnemonic) : null;
        }

        public static ViewModelConfig GetViewModelConfig(this HtmlHelper htmlHelper, string mnemonic)
        {
            var baseController = htmlHelper.ViewContext.Controller as IBaseController;

            return baseController != null ? baseController.GetViewModelConfig(mnemonic) : null;
        }

        public static IReadOnlyList<ViewModelConfig> GetViewModelConfigs(this HtmlHelper htmlHelper)
        {
            var baseController = htmlHelper.ViewContext.Controller as IBaseController;

            return baseController != null ? baseController.ViewModelConfigs : new List<ViewModelConfig>();
        }

        public static Menu GetMenu(this HtmlHelper htmlHelper)
        {
            var baseController = htmlHelper.ViewContext.Controller as IBaseController;

            return baseController != null ? baseController.GetMenu() : null;
        }

        public static IDictionary<string, object> CreateHtmlAttributes(this HtmlHelper htmlHelper, EditorViewModel eViewModel, object htmlAttributes = null, bool textbinding = false)
        {

            if (eViewModel == null)
            {
                throw new Exception("EditorViewModel is null");
            }

            var attributes = InitAttributes(htmlAttributes);

            var type = eViewModel.EditorType;

            string propertyName = eViewModel.PropertyName;

            if (typeof(MultilanguageText).IsAssignableFrom(type))
            {
                propertyName += ".Lang.ru";
            }

            if (!textbinding)
            {
                if (!attributes.ContainsKey("data-bind"))
                {
                    if (type == typeof(bool) || type == typeof(bool?))
                        attributes.Add("data-bind", String.Format("checked:{0}", propertyName));
                    else
                        attributes.Add("data-bind", String.Format("value:{0}", propertyName));
                }

                if (eViewModel.IsReadOnly)
                {
                    if (attributes.ContainsKey("class"))
                    {
                        attributes["class"] = attributes["class"] + " k-state-disabled";
                    }
                    else
                    {
                        attributes.Add("class", "k-state-disabled");
                    }

                    attributes.Add("disabled", "disabled");
                }
                else
                {

                    if (eViewModel.IsRequired)
                    {
                        attributes.Add("required", true);
                        attributes.Add("validationMessage", "Обязательное поле");
                    }
                }

            }
            else
            {
                if (!attributes.ContainsKey("data-bind"))
                {
                    attributes.Add("data-bind", String.Format("text:{0}", propertyName));
                }

            }

            return attributes;
        }

        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, EditorViewModel eViewModel, object htmlAttributes = null)
        {
            var tag = new TagBuilder("input");

            var attributes = htmlHelper.CreateHtmlAttributes(eViewModel, htmlAttributes);

            attributes.Add("id", eViewModel.UID);
            attributes.Add("type", "checkbox");
            attributes.Add("class", "k-checkbox");

            tag.MergeAttributes(new RouteValueDictionary(attributes));

            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, EditorViewModel eViewModel, object htmlAttributes = null)
        {
            var tag = new TagBuilder("input");

            var attributes = htmlHelper.CreateHtmlAttributes(eViewModel, htmlAttributes);

            if (attributes.ContainsKey("class"))
                attributes["class"] = attributes["class"] + " k-textbox";
            else
                attributes.Add("class", "k-textbox");

            tag.MergeAttributes(new RouteValueDictionary(attributes));

            return tag.ToMvcHtmlString(eViewModel);
        }

        public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, EditorViewModel eViewModel, object htmlAttributes = null)
        {
            var tag = new TagBuilder("textarea");

            var attributes = htmlHelper.CreateHtmlAttributes(eViewModel, htmlAttributes);

            if (attributes.ContainsKey("class"))
                attributes["class"] = attributes["class"] + " k-textbox";
            else
                attributes.Add("class", "k-textbox");


            tag.MergeAttributes(new RouteValueDictionary(attributes));
            
            return tag.ToMvcHtmlString(eViewModel);
        }

        private static MvcHtmlString ToMvcHtmlString(this TagBuilder tag, EditorViewModel eViewModel)
        {
            if (eViewModel.IsRequired)
            {
                string name = eViewModel.PropertyName;

                tag.MergeAttribute("id", eViewModel.UID);
                tag.MergeAttribute("name", name);

                var validationmsg = new TagBuilder("span");

                validationmsg.AddCssClass("k-invalid-msg");
                validationmsg.MergeAttribute("data-for", name);

                return MvcHtmlString.Create(tag.ToString() + validationmsg.ToString());
            }

            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString Span(this HtmlHelper htmlHelper, EditorViewModel eViewModel, object htmlAttributes = null)
        {
            var attributes = InitAttributes(htmlAttributes);

            if (!attributes.ContainsKey("data-bind"))
            {
                attributes.Add("data-bind", String.Format("text:{0}", eViewModel.PropertyName));
            }

            var tag = new TagBuilder("span");

            tag.MergeAttributes(new RouteValueDictionary(attributes));

            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString Action(this HtmlHelper htmlHelper, EditorViewModel eViewModel, object htmlAttributes = null)
        {
            var attributes = InitAttributes(htmlAttributes);

            string name = eViewModel.PropertyName;

            if (!attributes.ContainsKey("data-bind"))
            {
                attributes.Add("data-bind", "attr: { href: " + name + "}; text: " + name + "");
            }

            attributes.Add("target", "_blank");

            var tag = new TagBuilder("a");

            tag.MergeAttributes(new RouteValueDictionary(attributes));

            return MvcHtmlString.Create(tag.ToString());
        }

        private static Dictionary<string, object> InitAttributes(object htmlAttributes)
        {
            var attributes = htmlAttributes as Dictionary<string, object>;

            if (attributes == null)
            {
                attributes = new Dictionary<string, object>();

                if (htmlAttributes != null)
                {
                    foreach (var pr in htmlAttributes.GetType().GetProperties())
                    {
                        attributes.Add(pr.Name.Replace("_", "-"), pr.GetValue(htmlAttributes));
                    }
                }
            }

            return attributes;
        }

        public static string GetImageSrc(this HtmlHelper helper, Guid imageID, int? width = null,
            int? height = null, string defImage = "NoImage", string type = "Crop")
        {
            return String.Format("/Files/GetImage/{0}?width={1}&height={2}&defImage={3}&type={4}",
                imageID.ToString("N"), width, height, defImage, type);
        }

        public static string GetImageSrc(this HtmlHelper helper, FileData image, int? width = null, int? height = null, string defImage = "NoImage", string type = "Crop")
        {
            Guid guid = image != null ? image.FileID : Guid.Empty;

            return GetImageSrc(helper, guid, width, height, defImage, type);
        }

        public static string GetImageSrc(this HtmlHelper helper, string id, int? width = null, int? height = null,
            string defImage = "NoImage", string type = "Crop")
        {
            Guid guid;

            if (!Guid.TryParse(id, out guid))
            {
                guid = Guid.Empty;
            }

            return GetImageSrc(helper, guid, width, height, defImage, type);
        }

        public static bool IsDebug(this HtmlHelper htmlHelper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public enum ResourceType
        {
            Js = 0,
            Css = 1
        }

        public static IHtmlString Resource(this HtmlHelper htmlHelper, Func<object, dynamic> template, ResourceType ResType)
        {
            if (htmlHelper.ViewContext.HttpContext.Items[ResType] != null) ((List<Func<object, dynamic>>)htmlHelper.ViewContext.HttpContext.Items[ResType]).Add(template);
            else htmlHelper.ViewContext.HttpContext.Items[ResType] = new List<Func<object, dynamic>>() { template };

            return new HtmlString(String.Empty);
        }

        public static IHtmlString RenderResources(this HtmlHelper htmlHelper, ResourceType ResType)
        {
            if (htmlHelper.ViewContext.HttpContext.Items[ResType] != null)
            {
                var resources = (List<Func<object, dynamic>>)htmlHelper.ViewContext.HttpContext.Items[ResType];

                foreach (var resource in resources)
                {
                    if (resource != null) htmlHelper.ViewContext.Writer.Write(resource(null));
                }
            }

            return new HtmlString(String.Empty);
        }

    }
}