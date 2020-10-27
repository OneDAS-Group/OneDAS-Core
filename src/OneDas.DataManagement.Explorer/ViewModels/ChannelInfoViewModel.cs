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
        private ChannelMetaInfo _channelMeta;

        #endregion

        #region Constructors

        public ChannelInfoViewModel(ChannelInfo channel, ChannelMetaInfo channelMeta)
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

        public string Unit => !string.IsNullOrWhiteSpace(_channelMeta.Unit) 
            ? _channelMeta.Unit 
            : _channel.Unit;

        public string Description
        {
            get { return _channelMeta.Description; }
            set { _channelMeta.Description = value; }
        }

        public string SpecialInfo
        {
            get { return _channelMeta.SpecialInfo; }
            set { _channelMeta.SpecialInfo = value; }
        }

        public ProjectInfo Parent => (ProjectInfo)_channel.Parent;

        public List<TransferFunction> TransferFunctions => _channelMeta.TransferFunctions.Any() 
            ? _channelMeta.TransferFunctions
            : _channel.TransferFunctions;

        public List<DatasetInfoViewModel> Datasets { get; private set; }

        #endregion
    }
}
