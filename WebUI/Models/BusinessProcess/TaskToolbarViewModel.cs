using Base;
using Base.BusinessProcesses.Entities;
using Base.UI;
using System;

namespace WebUI.Models.BusinessProcess
{
    public class TaskToolbarViewModel
    {
        public int TaskID { get; set; }
        public string InfoString { get; set; }
        public string Title { get; set; }
        public string Mnemonic { get; set; }
        public int ObjectID { get; set; }
        public bool DisableStatus { get; set; }
        public bool IconOnly { get; set; }
        public ViewModelConfig Config { get; set; }

        public TaskToolbarViewModel(ViewModelConfig config, ChangeHistory historyItem, BaseObject baseObject)
        {
            Mnemonic = config.Mnemonic;
            Config = config;
            Title = config.Title;
            ObjectID = historyItem.ObjectID;
            InfoString = String.Format("{0}: {1}", Title, baseObject.GetType().GetProperty(config.LookupProperty).GetValue(baseObject));
        }
    }
}