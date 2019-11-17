using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneDas.Extensibility
{
    public abstract class DataGatewayExtensionLogicBase : ExtensionLogicBase
    {
        #region "Fields"

        private Stopwatch _lastSuccessfulUpdate;

        #endregion

        #region "Constructors"

        public DataGatewayExtensionLogicBase(DataGatewayExtensionSettingsBase settings) : base(settings)
        {
            this.Settings = settings;
        }

        #endregion

        #region "Properties"

        public new DataGatewayExtensionSettingsBase Settings { get; private set; }

        public Stopwatch LastSuccessfulUpdate
        {
            get
            {
                if (_lastSuccessfulUpdate == null)
                {
                    _lastSuccessfulUpdate = new Stopwatch();
                }

                return _lastSuccessfulUpdate;
            }
        }

        protected DataGatewayContext DataGatewayContext { get; private set; }

        #endregion

        #region "Methods"

        public abstract IEnumerable<DataPort> GetDataPortSet();

        public void Configure(DataGatewayContext dataGatewayContext)
        {
            this.DataGatewayContext = dataGatewayContext;
            this.OnConfigure();
        }

        public void UpdateIo(DateTime referenceDateTime)
        {
            this.OnUpdateIo(referenceDateTime);
        }

        protected virtual void OnConfigure()
        {
            //
        }

        protected virtual void OnUpdateIo(DateTime referenceDateTime)
        {
            //
        }

        #endregion
    }
}