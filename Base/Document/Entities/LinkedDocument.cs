using Base.LinkedObjects;
using Base.Security.ObjectAccess;
using Base.Security.ObjectAccess.Policies;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Document.Entities
{
    [AccessPolicy(typeof(EditCreatorOnlyAccessPolicy))]
    [Table("Documents")]
    public class LinkedDocument : DocumentBase, IAccessibleObject, ILinkedObject
    {
    }
}
