using System.Collections.Concurrent;
using System.Threading;

namespace OneDas.DataManagement.Explorer.Services
{
    public class FileAccessManager
    {
        #region Fields

        private ConcurrentDictionary<string, AutoResetEvent> _files;

        #endregion

        #region Constructors

        public FileAccessManager()
        {
            _files = new ConcurrentDictionary<string, AutoResetEvent>();
        }

        #endregion

        #region Methods

        public void Register(string filePath, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
            {
                foreach (var entry in _files)
                {
                    entry.Value.Set();
                }
            });

            while (true)
            {
                if (_files.TryGetValue(filePath, out var autoResetEvent))
                {
                    autoResetEvent.WaitOne();
                    cancellationToken.ThrowIfCancellationRequested();

                    continue;
                }

                _files[filePath] = new AutoResetEvent(initialState: false);

                break;
            }
        }

        public void Unregister(string filePath)
        {
            _files.TryRemove(filePath, out var autoResetEvent);
            autoResetEvent.Set();
        }

        #endregion
    }
}
