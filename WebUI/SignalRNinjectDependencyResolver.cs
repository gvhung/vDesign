using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Ninject;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using WebUI.Hubs;

namespace WebUI
{
    public class SignalRNinjectDependencyResolver : NinjectDependencyResolver
    {
        private readonly IKernel _kernel;

        public SignalRNinjectDependencyResolver(IKernel kernel) : base(kernel)
        {
            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            object _obj;
            var jj = "";

            if (serviceType == typeof(ConferenceHub))
                jj = "temp";

            if (_kernel.GetBindings(serviceType).Any())
            {
                _obj = _kernel.TryGet(serviceType, new IParameter[0]);
            }
            else
            {
                _obj = base.GetService(serviceType);
            }

            return _obj;
        }
    }

    public class MyDateTimeConvertor : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.ParseExact(reader.Value.ToString(), "dd.MM.yyyy HH:mm:ss", null);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("dd.MM.yyyy HH:mm:ss"));
        }

        public override bool CanConvert(Type objectType)
        {
            return base.CanConvert(objectType);
        }

        public override bool CanRead
        {
            get { return base.CanRead; }
        }

        public override bool CanWrite
        {
            get { return base.CanWrite; }
        }
    }

}