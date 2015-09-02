using System.Collections.Generic;
using System.Linq;

namespace WebUI.Hubs
{
    public class ConnectionMapping
    {
        private readonly List<ConnectionUser> _connections = new List<ConnectionUser>();

        public void Add(string userName, string connectionId)
        {
            lock (_connections)
            {
                if (_connections.All(x => x.ConnectionId != connectionId))
                {
                    _connections.Add(new ConnectionUser { UserName = userName, ConnectionId = connectionId });
                }
            }
        }

        public void Register(string userName, string connectionId)
        {
            lock (_connections)
            {
                foreach (var c in _connections.Where(x => x.ConnectionId == connectionId))
                {
                    c.UserName = userName;
                }
            }
        }

        public IEnumerable<string> GetConnections(string userName)
        {
            lock (_connections)
            {
                if (_connections.Any(x => x.UserName == userName)) 
                    return _connections.Select(x => x.ConnectionId);
            }
            return Enumerable.Empty<string>();
        }

        public void Remove(string userName, string connectionId)
        {
            lock (_connections)
            {
                _connections.RemoveAll(x => x.UserName == userName && x.ConnectionId == connectionId);
            }
        }
    }

    public class ConnectionUser
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}