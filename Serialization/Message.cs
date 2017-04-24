using System;
using ProtoBuf;
using SharpChannels.Core.Messages;

namespace Examples.Serialization
{
    [Serializable]
    [ProtoContract]
    public class Message : IMessage
    {
        [ProtoMember(1)]
        public string StringField { get; set; }

        [ProtoMember(2)]
        public int IntField { get; set; }

        [ProtoMember(3)]
        public DateTime DateTimeField { get; set; }

        public override string ToString()
        {
            return $"{StringField} {IntField} {DateTimeField}";
        }
    }
}