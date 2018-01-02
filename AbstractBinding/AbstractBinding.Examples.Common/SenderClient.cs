using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using AbstractBinding.Messages;
using AbstractBinding.Examples.Protos;

namespace AbstractBinding.Examples
{
    public class SenderClient : Protos.RecipientService.RecipientServiceClient, IProxyClient
    {
        private Serializer _serializer = new Serializer();
        private Task _listenTask;
        private CancellationTokenSource _listenCancel;

        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public SenderClient(string target) : base(new Channel(target, ChannelCredentials.Insecure))
        {
        }

        public void StartListener()
        {
            _listenCancel = new CancellationTokenSource();
            _listenTask = Task.Run(async () =>
            {
                AsyncServerStreamingCall<NotificationMessage> notificationStream = Listen(new Empty(), cancellationToken: _listenCancel.Token);
                while (!_listenCancel.Token.IsCancellationRequested)
                {
                    bool hasNotification = await notificationStream.ResponseStream.MoveNext(_listenCancel.Token);
                    if (hasNotification)
                    {
                        OnNotificationReceived(notificationStream.ResponseStream.Current.Message);
                    }
                }
            }, _listenCancel.Token);
        }

        public void StopListener()
        {
            _listenCancel.Cancel();
        }

        public IResponse Request(IRequest request)
        {
            ResponseMessage resp = this.Request(new RequestMessage() { Message = _serializer.SerializeObject(request) });
            return _serializer.DeserializeObject<IResponse>(resp.Message);
        }

        private void OnNotificationReceived(string notification)
        {
            NotificationReceived?.Invoke(this, new NotificationEventArgs(_serializer.DeserializeObject<INotification>(notification)));
        }
    }
}
