using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public abstract class HdfElementBase
    {
        #region "Fields"

        Dictionary<string, SyncContext> _syncMap;

        #endregion

        #region "Constructors"

        public HdfElementBase(string name, HdfElementBase parent, bool isLazyLoading)
        {
            this.Name = name;
            this.Parent = parent;
            this.IsLazyLoading = isLazyLoading;

            _syncMap = new Dictionary<string, SyncContext>();
        }

        #endregion

        #region "Properties"

        [DataMember]
        public string Name { get; set; }

        public bool IsLazyLoading { get; }

        public HdfElementBase Parent { get; set; }

        protected bool HasLoadedOnce { get; private set; }

        #endregion

        #region "Methods"

        public void Update(FileContext fileContext)
        {
            bool update;

            update = true;

            if (this.IsLazyLoading && !this.HasLoadedOnce)
            {
                if (_syncMap.Any(entry => entry.Value.FileContext != null))
                {
                    this.Update();
                }
                else
                {
                    update = false;
                }
            }

            _syncMap.ToList().ForEach(entry => entry.Value.FileContext = fileContext);

            if (update)
            {
                this.Update();
            }
        }

        /// <summary>
        /// After update is called the first time, the instance is not lazy loading anymore.
        /// </summary>
        public void Update()
        {
            long id;
            FileContext fileContext;
            List<SyncContext> syncContextSet;

            syncContextSet = _syncMap.ToList().Where(entry => entry.Value.FileContext != null).Select(entry => entry.Value).ToList();

            if (syncContextSet.Any())
            {
                fileContext = syncContextSet[0].FileContext;
                id = this.GetId(fileContext.FileId);

                try
                {
                    syncContextSet.ForEach(syncContext => syncContext.Action.Invoke(fileContext, id));
                }
                finally
                {
                    this.CloseId(id);
                }
            }

            this.HasLoadedOnce = true;

            syncContextSet.ForEach(syncContext => syncContext.FileContext = null);
        }

        protected void Update(string syncGroup)
        {
            long id;

            if (_syncMap.ContainsKey(syncGroup) && _syncMap[syncGroup].FileContext != null)
            {
                id = this.GetId(_syncMap[syncGroup].FileContext.FileId);

                try
                {
                    _syncMap[syncGroup].Action.Invoke(_syncMap[syncGroup].FileContext, id);
                    _syncMap[syncGroup].FileContext = null;
                }
                finally
                {
                    this.CloseId(id);
                }
            }
        }

        protected void RegisterSyncGroup(string syncGroup, SyncContext syncContext)
        {
            Contract.Requires(syncContext != null);

            _syncMap[syncGroup] = syncContext;
        }

        public abstract string GetPath();
        public abstract string GetDisplayName();
        public abstract IEnumerable<HdfElementBase> GetChildSet();
        protected abstract long GetId(long fileId);
        protected abstract void CloseId(long id);

        #endregion
    }
}
