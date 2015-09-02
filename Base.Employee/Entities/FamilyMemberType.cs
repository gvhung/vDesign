using Base.Attributes;

namespace Base.Employee.Entities
{
    public class FamilyMemberType : BaseObject
    {
        [ListView]
        [DetailView("Наименование")]
        public string Title { get; set; }
    }
}