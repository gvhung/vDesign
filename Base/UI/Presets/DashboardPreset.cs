using Base.Attributes;
using System;
using System.Collections.Generic;

namespace Base.UI.Presets
{
    [Serializable]
    public class DashboardPreset: Preset
    {
        public DashboardPreset()
        {
            Widgets = new List<DashboardWidget>();
        }

        [PropertyDataType("DashboardWidgets")]
        [DetailView("Виджеты")]
        public List<DashboardWidget> Widgets { get; set; }
    }
}
