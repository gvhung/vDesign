using Base.Attributes;
using System;
using System.Linq;

namespace Base.Censorship.Entities
{
    [Serializable]
    public class CensorshipConfig : BaseObject
    {
        [NonSerialized]
        private string[] _whiteListArray = null;

        [DetailView("Установить цензуру")]
        public bool TurnOn { get; set; }

        [DetailView("Регулярное выражение", Height = 25, Description = "Сервисы для проверки: debuggex.com или regexper.com")]
        public string Regex { get; set; }

        [DetailView("Белый список", Description = "Слова-исключения, разделитель - пробел")]
        public string WhiteList { get; set; }
        
        public string[] WhiteListArray 
        {
            get
            {
                if (_whiteListArray == null)
                    _whiteListArray = this.WhiteList.Split(' ', ';').Where(x => !String.IsNullOrEmpty(x)).ToArray();

                return _whiteListArray;
            }
        }
    }
}
