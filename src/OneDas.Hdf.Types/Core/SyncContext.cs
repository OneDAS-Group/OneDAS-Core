using System;

namespace OneDas.Hdf.Core
{
    public class SyncContext
    {
        #region "Constructors"

        public SyncContext(Action<FileContext, long> action)
        {
            this.Action = action;
        }

        #endregion

        #region "Properties"

        public Action<FileContext, long> Action { get; }
        public FileContext FileContext { get; set; }

        #endregion
    }
}
