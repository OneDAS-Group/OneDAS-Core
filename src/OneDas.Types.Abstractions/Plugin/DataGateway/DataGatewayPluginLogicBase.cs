using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneDas.Plugin
{
    public abstract class DataGatewayPluginLogicBase : PluginLogicBase
    {
        private Stopwatch _lastSuccessfulUpdate;

        public DataGatewayPluginLogicBase(DataGatewayPluginSettingsBase settings) : base(settings)
        {
            this.Settings = settings;
        }

        public new DataGatewayPluginSettingsBase Settings { get; private set; }
        
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

        public abstract IEnumerable<DataPort> GetDataPortSet();
        public abstract void Configure();
        public abstract void UpdateIo(DateTime referenceDateTime);
    }
}