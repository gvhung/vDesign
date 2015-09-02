using Framework.Maybe;
using System;

namespace Base.UI
{
    [Serializable]
    public class DashboardWidget
    {
        public DashboardWidget() { }

        public DashboardWidget(ViewModelConfig config)
        {
            this.Icon = config.Icon;
            this.Mnemonic = config.Mnemonic;
            this.Title = config.With(x => x.DetailView.Title) ?? config.Title;
            this.PanelName = config.With(x => x.ListView.Name) ?? "main";
        }

        public bool Hidden { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Mnemonic { get; set; }
        public string PanelName { get; set; }
        public string ViewName
        {
            get
            {
                return this.Mnemonic.Replace("DashboardWidget_", "");
            }
        }
    }
}
