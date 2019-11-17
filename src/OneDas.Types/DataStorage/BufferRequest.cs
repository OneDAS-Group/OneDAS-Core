using OneDas.Infrastructure;
using System.Runtime.Serialization;

namespace OneDas.DataStorage
{
    [DataContract]
    public class BufferRequest
    {
        public BufferRequest()
        {
            //
        }

        [DataMember]
        public SampleRate SampleRate { get; private set; }

        [DataMember]
        public string GroupFilter { get; private set; }
    }
}