using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Base.Content
{
    [ComplexType]
    public class Content
    {
        [Column]
        private byte[] value_ { get; set; }
        public string Source { get; set; }
        public string Html { get; set; }

        [NotMapped]
        public bool HasInteractive
        {
            get { return value_ != null; }
        }

        private List<ContentResult> _results;
        [NotMapped]
        public List<ContentResult> Results
        {
            get
            {
                if (_results == null && value_ != null)
                {
                    using (var ms = new MemoryStream(value_))
                    {
                        var bin = new BinaryFormatter();

                        return (_results = bin.Deserialize(ms) as List<ContentResult>);
                    }
                }

                return _results ?? new List<ContentResult>(0);
            }
            set
            {
                using (var ms = new MemoryStream())
                {
                    var bin = new BinaryFormatter();

                    bin.Serialize(ms, value);

                    value_ = ms.ToArray();

                    _results = null;
                }
            }
        }
    }

    [Serializable]
    public class ContentResult
    {
        public string UID { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
