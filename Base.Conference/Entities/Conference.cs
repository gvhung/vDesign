using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Security;
using Newtonsoft.Json;

namespace Base.Conference.Entities
{
    public class Conference : BaseObject
    {
        public string Title { get; set; }

        public virtual ICollection<ConferenceMember> Members { get; set; }

        public int CreatorId { get; set; }

        [JsonIgnore]
        [ForeignKey("CreatorId")]
        public virtual User Creator { get; set; }

        public DateTime CreateDate { get; set; }

        [JsonIgnore]
        [InverseProperty("ToConference")]
        public virtual List<PublicMessage> Messages { get; set; } 

    }

    public class ConferenceMember : EasyCollectionEntry<User>
    {
        
    }

}
