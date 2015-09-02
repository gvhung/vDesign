using System.Collections.Generic;

namespace WebUI.Models.BusinessProcess
{
    public class BPActionBuilderVm
    {
        public string Title { get; set; }
        public string Member { get; set; }
        public IEnumerable<BPActionValueVm> Values { get; set; }
    }

    public class BPActionValueVm
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }
}