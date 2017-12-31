using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractBinding.Messages;

namespace AbstractBinding
{
    public interface IRecipientCallback
    {
        void Callback(INotification notification);
    }
}

