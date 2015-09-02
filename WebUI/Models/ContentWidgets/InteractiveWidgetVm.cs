using Base;
using Base.Attributes;
using System;
using System.Collections.Generic;

namespace WebUI.Models.ContentWidgets
{
    public class InteractiveWidgetVm : WidgetVm
    {
    }

    public class TestWidget<TResultType> : InteractiveWidgetVm
        where TResultType : TestWidgetItem, new()
    {
        [DetailView("Заголовок")]
        [PropertyDataType(PropertyDataType.SimpleHtml)]
        public string Title { get; set; }
        [DetailView("Значения")]
        public List<TResultType> Values { get; set; }

        public TestWidget()
        {
            this.Title = "- введите вопрос -";
            this.Values = new List<TResultType>
            {
                new TResultType { Title = "Ответ 1" },
                new TResultType { Title = "Ответ 2" }
            };
        }
    }

    public class TestWidgetItem : BaseObject
    {
        private static Random _rnd = new Random();

        public string UID { get; set; }
        
        [DetailView("Вопрос")]
        [ListView]
        public string Title { get; set; }

        public TestWidgetItem()
        {
            this.ID = _rnd.Next(0, 99999);
            this.UID = Guid.NewGuid().ToString("N");
            this.Title = "- введити ответ -";
        }
    }

    public class CheckboxTestItem : TestWidgetItem
    {
        [ListView]
        [DetailView("Верно")]
        public bool Value { get; set; }
    }
}