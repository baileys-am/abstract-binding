using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using AbstractBinding.Examples.Protos;

namespace AbstractBinding.Examples
{
    public class RecipientService : Protos.RecipientService.RecipientServiceBase, IRecipientCallback
    {
        private readonly Recipient _recipient;
        private readonly Queue<string> _notificationQueue = new Queue<string>();
        private readonly object _notificationLock = new object();

        public RecipientService(Recipient recipient)
        {
            _recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        }

        public async override Task<ResponseMessage> Request(RequestMessage request, ServerCallContext context)
        {
            return await Task.FromResult(new ResponseMessage() { Message = _recipient.Request(request.Message, this) });
        }

        public async override Task Listen(Empty request, IServerStreamWriter<NotificationMessage> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                string notification = null;
                lock (_notificationLock)
                {
                    if (_notificationQueue.Any())
                    {
                        notification = _notificationQueue.Dequeue();
                    }
                }

                if (notification != null)
                {
                    await responseStream.WriteAsync(new NotificationMessage() { Message = notification });
                }
                else
                {
                    await Task.Delay(50);
                }
            }
        }

        public void Callback(string notification)
        {
            lock (_notificationLock)
            {
                _notificationQueue.Enqueue(notification);
            }
        }
    }
}