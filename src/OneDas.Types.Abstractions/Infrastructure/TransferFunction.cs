using System;
using System.Runtime.Serialization;

namespace OneDas.Infrastructure
{
    [DataContract]
    public class TransferFunction
    {
        #region "Constructors"

        public TransferFunction(DateTime dateTime, string type, string option, string argument)
        {
            this.DateTime = dateTime;
            this.Type = type;
            this.Option = option;
            this.Argument = argument;
        }

        #endregion

        #region "Properties"

        [DataMember]
        public DateTime DateTime { get; private set; }

        [DataMember]
        public string Type { get; private set; }

        [DataMember]
        public string Option { get; private set; }

        [DataMember]
        public string Argument { get; private set; }

        #endregion
    }
}