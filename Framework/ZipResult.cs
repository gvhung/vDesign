using Ionic.Zip;
using Ionic.Zlib;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Framework
{
    public class ZipResult : ActionResult
    {
        private Dictionary<string, string> _files;
        private Dictionary<string, string> _directories;
        private string _fileName;
        private ZipResult_CompressionLevel _compressionLevel = ZipResult_CompressionLevel.Default;
        
        public string FileName
        {
            get
            {
                return _fileName ?? "archivo.zip";
            }
            set { _fileName = value; }
        }

        public ZipResult(Dictionary<string, string> files)
        {
            this._files = files;
        }

        public ZipResult(Dictionary<string, string> files, Dictionary<string, string> directories)
        {
            this._files = files;
            this._directories = directories;
        }

        public ZipResult(Dictionary<string, string> files, Dictionary<string, string> directories, ZipResult_CompressionLevel compressionLevel)
        {
            this._files = files;
            this._directories = directories;
            this._compressionLevel = compressionLevel;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            using (ZipFile zf = new ZipFile(System.Text.Encoding.GetEncoding("cp866")))
            {
                zf.CompressionLevel = (CompressionLevel)this._compressionLevel;

                foreach (var file in _files)
                {
                    string val = file.Value;

                    int step = 1;
                    
                    while(zf.ContainsEntry(val))
                    {
                        val = step + "_" + val;

                        step++;
                    }

                    zf.AddFile(file.Key).FileName = val;
                }

                foreach (var dir in _directories)
                {
                    string val = dir.Value;

                    int step = 1;

                    while (zf.ContainsEntry(val))
                    {
                        val = step + "_" + val;

                        step++;
                    }
                    
                    zf.AddDirectory(dir.Key, val);
                }
                
                context.HttpContext
                    .Response.ContentType = "application/zip";
                context.HttpContext
                    .Response.AppendHeader("content-disposition", "attachment; filename=" + FileName);
                zf.Save(context.HttpContext.Response.OutputStream);
            }
        }

    }

  
    public enum ZipResult_CompressionLevel
    {
        Level0 = 0,
        None = 0,
        Level1 = 1,
        BestSpeed = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6,
        Default = 6,
        Level7 = 7,
        Level8 = 8,
        BestCompression = 9,
        Level9 = 9,
    }
}
