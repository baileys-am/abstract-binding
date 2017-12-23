using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    public interface IRecipientCallback
    {
        void Callback(string notification);
    }
}

