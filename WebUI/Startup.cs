using System;
using System.IO;
using System.Web.Helpers;
using Base;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Ninject;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;
using Owin;

[assembly: OwinStartup(typeof(WebUI.Startup))]

namespace WebUI
{
    public class Startup
    {
        private static StandardKernel _kernel;

        public void Configuration(IAppBuilder app)
        {
            var resolver = new NinjectDependencyResolver(_kernel);

            //var jsonSerializer = new JsonSerializer()
            //{
            //    DateFormatString = "dd.MM.yyyy HH:mm:ss",
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            //};

            var settings = new JsonSerializerSettings()
            {
                DateFormatString = "dd.MM.yyyy HH:mm:ss",
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            };

            settings.Converters.Add(new MyDateTimeConvertor());
            //jsonSerializer.Converters.Add(new MyDateTimeConvertor());

            //var myfile = "{\"FileID\":\"16e0cfae-bd15-41fc-9608-40e49888825a\",\"FileName\":\"6770261-amazing-redhead.jpg\",\"Size\":644995,\"CreationDate\":\"13.08.2015 15:13:01\",\"ChangeDate\":\"13.08.2015 15:13:01\",\"Key\":\"16e0cfae-bd15-41fc-9608-40e49888825a\",\"ID\":0,\"Hidden\":false,\"SortOrder\":-1,\"RowVersion\":null}";
            //var data = JsonConvert.DeserializeObject<FileData>(myfile, settings);


            resolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(settings));

            app.MapSignalR("/signalr", new HubConfiguration()
            {
                Resolver = resolver,
                EnableJSONP = true,
                EnableDetailedErrors = true,
            });

            //GlobalHost.DependencyResolver = resolver;
            //GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(settings));
        }

        public static StandardKernel CreateKernel()
        {
            if (_kernel == null)
            {
                _kernel = new StandardKernel();
                _kernel.Load(new BindingsWithEvents());
            }

            return _kernel;
        }

    }
}