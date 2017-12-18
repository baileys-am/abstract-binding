using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.SenderInternals
{
    internal class RuntimeProxy : IDisposable
    {
        private readonly IAbstractClient _client;
        private readonly ISerializer _serializer;
        private readonly Dictionary<string, EventHandler> _eventHandlers = new Dictionary<string, EventHandler>();

        public RuntimeProxy(IAbstractClient client, ISerializer serializer)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public bool Subscribe(Type interfaceType, string name, EventHandler result)
        {
            _eventHandlers.Add(name, result);
            return true;
        }

        public bool Unsubscribe(Type interfaceType, string name, EventHandler result)
        {
            _eventHandlers.Remove(name);
            return true;
        }

        public bool SetValue(Type interfaceType, string name, object value)
        {
            return true;
        }

        public bool GetValue(Type interfaceType, string name, out object value)
        {
            value = interfaceType.GetProperty(name).GetMethod.ReturnType.GetDefault();
            return true;
        }

        public bool Invoke(Type interfaceType, string name, object[] args, out object result)
        {
            result = interfaceType.GetMethod(name).ReturnType.GetDefault();
            return true;
        }

        public void OnNotify(string name)
        {
            if (_eventHandlers.ContainsKey(name))
            {
                _eventHandlers[name]?.Invoke(this, EventArgs.Empty);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RuntimeObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
