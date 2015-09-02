using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebUI.Helpers
{
    public class CustomDateTimeConvertor : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.ParseExact(reader.Value.ToString(), "dd.MM.yyyy HH:mm:ss", null);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("dd.MM.yyyy HH:mm:ss"));
        }
    }
}