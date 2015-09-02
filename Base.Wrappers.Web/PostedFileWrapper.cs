using Framework.Wrappers;
using System.Web;

namespace Base.Wrappers.Web
{
    public class PostedFileWrapper : IPostedFileWrapper
    {
        private HttpPostedFileBase _item;

        public void SetItem(object obj)
        {
            this._item = obj as HttpPostedFileBase;
        }

        public int ContentLength
        {
            get { return this._item.ContentLength; }
        }

        public string ContentType
        {
            get { return this._item.ContentType; }
        }

        public string FileName
        {
            get { return this._item.FileName; }
        }

        public System.IO.Stream InputStream
        {
            get { return this._item.InputStream; }
        }

        public void SaveAs(string filename)
        {
            this._item.SaveAs(filename);
        }
    }
}