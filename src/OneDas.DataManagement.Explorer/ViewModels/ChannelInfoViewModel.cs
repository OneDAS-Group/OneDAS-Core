using OneDas.DataManagement.Database;
using OneDas.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace OneDas.DataManagement.Explorer.ViewModels
{
    public class ChannelInfoViewModel
    {
        #region Fields

        private ChannelInfo _channel;
        private ChannelMeta _channelMeta;

        #endregion

        #region Constructors

        public ChannelInfoViewModel(ChannelInfo channel, ChannelMeta channelMeta)
        {
            _channel = channel;
            _channelMeta = channelMeta;

            this.Datasets = channel.Datasets
                .Where(dataset => !dataset.Id.EndsWith("_status"))
                .Select(dataset => new DatasetInfoViewModel(dataset, this)).ToList();
        }

        #endregion

        #region Properties

        public string Id => _channel.Id;

        public string Name => _channel.Name;

        public string Group => _channel.Group;

        public string Unit
        {
            get
            { 
                return !string.IsNullOrWhiteSpace(_channelMeta.Unit)
                    ? _channelMeta.Unit
                    : _channel.Unit;
            }
            set 
            {
                _channelMeta.Unit = value; 
            }
        }

        public string Description
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_channelMeta.Description)
                    ? _channelMeta.Description
                    : _channel.Description;
            }
            set
            {
                _channelMeta.Description = value;
            }
        }

        public string SpecialInfo
        {
            get { return _channelMeta.SpecialInfo; }
            set { _channelMeta.SpecialInfo = value; }
        }

        public ProjectInfo Parent => (ProjectInfo)_channel.Parent;

        public List<DatasetInfoViewModel> Datasets { get; private set; }

        #endregion
    }
}
