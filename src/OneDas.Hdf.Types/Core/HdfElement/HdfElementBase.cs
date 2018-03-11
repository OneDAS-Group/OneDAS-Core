using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OneDas.Hdf.Core
{
    [DataContract]
    public abstract class HdfElementBase
    {
        #region "Fields"

        private FileContext _cachedFileContext;

        #endregion

        #region "Constructors"

        public HdfElementBase(string name, HdfElementBase parent, bool isLazyLoading)
        {
            this.Name = name;
            this.Parent = parent;
            this.IsLazyLoading = isLazyLoading;
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

        public void Update()
        {
            if (_cachedFileContext != null)
            {
                this.OnUpdate(_cachedFileContext);
                this.HasLoadedOnce = true;

                _cachedFileContext = null;
            }
        }

        public void Update(FileContext fileContext)
        {
            if (this.IsLazyLoading && !this.HasLoadedOnce)
            {
                if (_cachedFileContext != null)
                {
                    this.OnUpdate(_cachedFileContext);
                    this.OnUpdate(fileContext);
                    this.HasLoadedOnce = true;

                    _cachedFileContext = null;
                }
                else
                {
                    _cachedFileContext = fileContext;
                }
            }
            else
            {
                this.OnUpdate(fileContext);
                this.HasLoadedOnce = true;
            }
        }

        public abstract string GetPath();
        public abstract string GetDisplayName();
        public abstract IEnumerable<HdfElementBase> GetChildSet();
        protected abstract void OnUpdate(FileContext fileContext);

        #endregion
    }
}
