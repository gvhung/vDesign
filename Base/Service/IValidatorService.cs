using Base.Politics;
using System;
using System.Collections.Generic;

namespace Base.Service
{
    public interface IValidatorService
    {
        void AddObjectPolitic<T>(T objectPolitic) where T : IObjectPolitic<BaseObject>;
        void AddPropertyPolitic<T>(T propertyPolitic) where T : IPropertyPolitic<Type>;
        ICollection<IObjectValidationResult> ValidateObject<T>(T Obj) where T : BaseObject; 
    }

    public class ValidatorService : IValidatorService
    {
        public ICollection<IObjectPolitic<BaseObject>> ObjectPolitics { get; set; }
        public ICollection<IPropertyPolitic<Type>> PropertyPolitics { get; set; }

        public void AddObjectPolitic<T>(T objectPolitic) where T : IObjectPolitic<BaseObject>
        {
            ObjectPolitics.Add(objectPolitic);
        }

        public void AddPropertyPolitic<T>(T propertyPolitic) where T : IPropertyPolitic<Type>
        {
            PropertyPolitics.Add(propertyPolitic);
        }

        public ICollection<IObjectValidationResult> ValidateObject<T>(T obj) where T : BaseObject
        {
            return  new List<IObjectValidationResult>();
        }
    }
}

