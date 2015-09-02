using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Base;
using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.UI;
using Base.UI.Service;

namespace WebUI.Concrete
{
    public class DetailViewSettingManager : IDetailViewSettingManager
    {
        public IEnumerable<DetailViewSetting> GetSettings(IUnitOfWork unitOfWork, string mnemonic, BaseObject obj)
        {
            if (obj == null) return null;

            var settings = new List<DetailViewSetting>();

            if (obj is IBPObject)
            {
                var wfctx = (obj as IBPObject).WorkflowContext;

                if (wfctx != null)
                {
                    var stages = wfctx.CurrentStages;

                    foreach (var stage in stages)
                    {
                        //TODO: необходимо обойти всю иерархию декомпозиции!!!
                        if (stage.Position != null && stage.Position.CurrentWorkflowContainer != null && stage.Position.CurrentWorkflowContainer.DetailViewSettingID != null)
                            settings.Add(stage.Position.CurrentWorkflowContainer.DetailViewSetting);

                        if (stage.Stage.DetailViewSettingID != null)
                            settings.Add(stage.Stage.DetailViewSetting);
                    }
                }
            }

            return settings;
        }
    }
}