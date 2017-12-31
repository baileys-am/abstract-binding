using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractBinding.Messages;

namespace AbstractBinding
{
    //internal class ProxyClient : IProxyClient
    //{
    //    private readonly ISenderClient _client;
    //    private readonly ISerializer _serializer;

    //    public event EventHandler<Messages.NotificationEventArgs> NotificationReceived;

    //    public ProxyClient(ISenderClient client, ISerializer serializer)
    //    {
    //        _client = client ?? throw new ArgumentNullException(nameof(client));
    //        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

    //        _client.NotificationReceived += _client_NotificationReceived;
    //    }

    //    ~ProxyClient()
    //    {
    //        _client.NotificationReceived -= _client_NotificationReceived;
    //    }

    //    private void _client_NotificationReceived(object sender, NotificationEventArgs e)
    //    {
    //        var notification = _serializer.DeserializeObject<Notification>(e.Notification);
    //        NotificationReceived?.Invoke(sender, new Messages.NotificationEventArgs(notification));
    //    }

    //    public IResponse Request(IRequest request)
    //    {
    //        var req = _serializer.SerializeObject(request);
    //        var resp = _client.Request(req);
    //        return _serializer.DeserializeObject<IResponse>(resp);
    //    }
    //}
}
