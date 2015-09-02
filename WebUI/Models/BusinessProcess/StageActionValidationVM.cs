using System;
using System.Collections.Generic;

namespace WebUI.Models.BusinessProcess
{
    public class StageActionValidationVm 
    {
        public ICollection<Type> Rules { get; set; }
        public string Property { get; set; }
    }
}