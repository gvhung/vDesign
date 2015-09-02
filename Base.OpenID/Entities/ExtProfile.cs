using Base.Attributes;
using Base.Security;
using System.Collections.Generic;

namespace Base.OpenID.Entities
{
    public class ExtProfile : Profile
    {
        [ListView]
        [DetailView(TabName = "[3]Внешние аккаунты", HideLabel = true)]
        [PropertyDataType("OpenID")]
        public IEnumerable<ExtAccount> AccountInfos { get; set; }
    }
}
