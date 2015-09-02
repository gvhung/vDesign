using Base.UI;
using System;
using System.Collections.Generic;

namespace WebUI.Models.Dashboard.Widgets
{
    public class CounterWidget : DashboardWidgetVm
    {
        public DashboardWidget Model { get; set; }

        public CounterWidget(DashboardWidget model)
        {
            Model = model;
        }

        public List<CounterMnemonicVm> Items { get; set; }
        public string ItemsTemplate { get; set; }
        public string CountUrl { get; set; }
        public string ItemsUrl { get; set; }
        public string AdditionalScript { get; set; }
        public string WrapID { get; set; }
        public string FirstElement { get; set; }
        public int? MaxItemsCount { get; set; }
    }

    public class CounterMnemonicVm
    {
        public string Mnemonic
        {
            get { return Config.Mnemonic; }
        }

        public string ID { get; set; }
        public string Color { get; set; }
        public ViewModelConfig Config { get; set; }

        public CounterMnemonicVm(ViewModelConfig viewModelConfig)
        {
            Config = viewModelConfig;
            ID = "x_" + Guid.NewGuid().ToString().Split('-')[0];
        }
    }
}