using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Base.Entities.Complex
{
    [EnableFullTextSearch]
    [ComplexType]
    public class MultilanguageText
    {
        public MultilanguageText()
        {
        }

        public MultilanguageText(string ru)
        {
            Dictionary<string, string> dic = this.Lang;
            dic["ru"] = ru;
            this.Lang = dic;
        }

        [FullTextSearchProperty]
        public string Xml { get;  set; }

        [NotMapped]
        [JsonIgnore]
        public XElement XmlValue
        {
            get { return Xml != null ? XElement.Parse(Xml) : null; }
            set { Xml = value != null ? value.ToString() : null; }
        }

        [NotMapped]
        public string this[string lang]
        {
            get
            {
                return this.Lang.ContainsKey(lang) ? this.Lang[lang] : this.Lang["ru"];
            }

            set
            {
                if (!this.Lang.ContainsKey(lang))
                {
                    this.Lang.Add(lang, value);
                }
                else
                {
                    this.Lang[lang] = value;
                }
            }
        }

        [NotMapped]
        public Dictionary<string, string> Lang
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                if (XmlValue != null)
                {
                    XElement el = this.XmlValue;

                    if (el != null)
                    {
                        foreach (XElement item in el.Elements())
                        {
                            dic.Add(item.Name.LocalName, item.Value);
                        }
                    }
                }

                if (!dic.ContainsKey("ru"))
                {
                    dic.Add("ru", "");
                }

                return dic;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    XmlValue = null;
                }
                else
                {
                    XElement xel = new XElement("root");

                    foreach (string key in value.Keys)
                    {
                        if (!String.IsNullOrEmpty(value[key]))
                        {
                            xel.Add(new XElement(key, value[key]));
                        }
                    }

                    XmlValue = xel;
                }
            }
        }

        public override string ToString()
        {
            return this.ToString("ru");
        }

        public string ToString(string lang)
        {
            return this[lang];
        }
    }

    [ComplexType]
    public class MultilanguageTextArea : MultilanguageText
    {
        public MultilanguageTextArea()
        {
        }

        public MultilanguageTextArea(string ru)
            : base(ru)
        {
        }
    }

    [ComplexType]
    public class MultilanguageHtml : MultilanguageText
    {
        public MultilanguageHtml()
        {
        }

        public MultilanguageHtml(string ru)
            : base(ru)
        {
        }
    }

    [ComplexType]
    public class Multilanguage<T>  where T: class
    {
        [FullTextSearchProperty]
        public string Value { get;  set; }

        [NotMapped]
        public T this[string lang]
        {
            get
            {
                return this.Lang.ContainsKey(lang) ? this.Lang[lang] : this.Lang["ru"];
            }

            set
            {
                if (!this.Lang.ContainsKey(lang))
                {
                    this.Lang.Add(lang, value);
                }
                else
                {
                    this.Lang[lang] = value;
                }
            }
        }

        private Dictionary<string, T> _lang;

        [NotMapped]
        public Dictionary<string, T> Lang
        {
            get
            {
                if (_lang == null)
                {
                    if (this.Value != null)
                    {
                        _lang =
                            JsonConvert.DeserializeObject(Value, typeof (Dictionary<string, T>)) as
                                Dictionary<string, T>;
                    }

                    if(_lang == null)
                        _lang = new Dictionary<string, T>();

                    if (!_lang.ContainsKey("ru"))
                    {
                        _lang.Add("ru", Activator.CreateInstance<T>());
                    }
                }

                return _lang;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    this.Value = null;
                    _lang = null;
                }
                else
                {
                    this.Value = JsonConvert.SerializeObject(value, Formatting.None);
                    _lang = value;
                }
            }
        }

        public override string ToString()
        {
            return this.ToString("ru");
        }

        public string ToString(string lang)
        {
            return this[lang].ToString();
        }
    }
}
