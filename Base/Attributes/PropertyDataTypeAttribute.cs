using System;

namespace Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyDataTypeAttribute : Attribute
    {
        public PropertyDataTypeAttribute(PropertyDataType dataType)
        {
            this.DataType = dataType;
        }
        public PropertyDataTypeAttribute(string dataType)
        {
            this.CustomDataType = dataType;
        }
        public PropertyDataType DataType { get; private set; }
        public string CustomDataType { get; private set; }
        public string Params { get; set; }
    }

    public enum PropertyDataType
    {
        Custom,
        DateTime,
        Date,
        Time,
        Month,
        Duration,
        PhoneNumber,
        Currency,
        Text,
        Html,
        SimpleHtml,
        MultilineText,
        EmailAddress,
        Password,
        Url,
        ImageUrl,
        CreditCard,
        PostalCode,
        Upload,
        Image,
        Number,
    }
}
