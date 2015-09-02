using Base;
using Base.DAL;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Data.Service.Concrete
{
    public class RequestLogService : BaseObjectService<RequestLog>, IRequestLogService, IReadOnly
    {
        private readonly IPathHelper _pathHelper;

        private const string filename = "request_log.xml";

        private readonly List<RequestLog> requestLog;
        private const int maxCount = 2000;
        private static DateTime lastSaveTime = DateTime.Now;

        private static readonly object _locker = new object();

        public RequestLogService(IBaseObjectServiceFacade facade, IPathHelper pathHelper)
            : base(facade)
        {
            _pathHelper = pathHelper;

            string fullName = GetXmlFullName();

            if (File.Exists(fullName))
            {
                try
                {
                    XmlSerializer reader = new XmlSerializer(typeof(List<RequestLog>));
                    StreamReader file = new StreamReader(GetXmlFullName());
                    requestLog = (List<RequestLog>)reader.Deserialize(file);
                }
                catch {}
            }

            if (requestLog == null)
                requestLog = new List<RequestLog>();
        }

        private string GetXmlFullName()
        {
            string directory = _pathHelper.GetLogDirectory();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, filename);
        }

        public override RequestLog Create(IUnitOfWork unitOfWork, RequestLog obj)
        {
            if (obj.Request.IndexOf("mnemonic=RequestLog", StringComparison.CurrentCultureIgnoreCase) != -1)
                return null;

            requestLog.Insert(0, obj);

            try
            {
                while (requestLog.Count() > maxCount)
                    requestLog.Remove(requestLog.Last());
            }
            catch {}

            return obj;
        }

        public override RequestLog Update(IUnitOfWork unitOfWork, RequestLog obj)
        {
            if (obj.Request.IndexOf("mnemonic=RequestLog", StringComparison.CurrentCultureIgnoreCase) != -1)
                return null;

            var request = requestLog.First(x => x.ID == obj.ID);
            request.End = obj.End;

            if ((DateTime.Now - lastSaveTime).TotalSeconds > 120)
            {
                lock (_locker)
                {
                    lastSaveTime = DateTime.Now;
                }

                try
                {
                    XmlSerializer writer = new XmlSerializer(typeof(List<RequestLog>));
                    FileStream file = File.Create(GetXmlFullName());
                    writer.Serialize(file, requestLog);
                    file.Close();
                }
                catch {}
            }

            return obj;
        }

        public override RequestLog Get(IUnitOfWork unitOfWork, int id)
        {
            return requestLog.FirstOrDefault(x => x.ID == id);
        }

        public override IQueryable<RequestLog> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return requestLog.AsQueryable();
        }
    }
}
