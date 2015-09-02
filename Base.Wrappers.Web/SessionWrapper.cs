using Framework.Wrappers;
using System.Web.SessionState;

namespace Base.Wrappers.Web
{
    public class SessionWrapper : ISessionWrapper
    {
        private HttpSessionState _item;

        public SessionWrapper(HttpSessionState session)
        {
            _item = session;
        }

        public object this[string name]
        {
            get
            {
                if (this._item == null) return null;
                return this._item[name];
            }
            set
            {
                if (this._item == null) return;
                this._item[name] = value;
            }
        }

        public void Remove(string key)
        {
            this._item.Remove(key);
        }

        public bool HasEntry
        {
            get { return _item != null; }
        }
    }
}