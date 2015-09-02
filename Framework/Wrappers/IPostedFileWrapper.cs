using System.IO;

namespace Framework.Wrappers
{
    public interface IPostedFileWrapper
    {
        void SetItem(object obj);
        int ContentLength { get; }
        string ContentType { get; }
        string FileName { get; }
        Stream InputStream { get; }
        void SaveAs(string filename);
    }
}
