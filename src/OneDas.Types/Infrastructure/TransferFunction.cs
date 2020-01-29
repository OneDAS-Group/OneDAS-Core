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
            if (dateTime.Kind != DateTimeKind.Utc)
                throw new Exception("DateTime instances must be in the UTC+0 format.");

            this.DateTime = dateTime;
            this.Type = type;
            this.Option = option;
            this.Argument = argument;
        }

        public TransferFunction()
        {
            //
        }

        #endregion

        #region "Properties"

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Option { get; set; }

        [DataMember]
        public string Argument { get; set; }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                var tf = (TransferFunction)obj;

                return this.DateTime.Equals(tf.DateTime) &&
                       this.Type == tf.Type &&
                       this.Option == tf.Option &&
                       this.Argument == tf.Argument;
            }
        }

        public override int GetHashCode()
        {
            return this.DateTime.GetHashCode() + this.Type.Length + this.Option.Length + this.Argument.Length;
        }

        #endregion
    }
}