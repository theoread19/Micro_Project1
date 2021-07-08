using Confluent.Kafka;
using Google.Protobuf;
using System;
using System.IO;
using UserProtoBufService;

namespace Infrastructure.Kafka.Producer
{
    internal class UserSerializer : ISerializer<UserProtoReq>
    {
        public byte[] Serialize(UserProtoReq data, SerializationContext context)
        {
            return data.ToByteArray();
        }      
    }
}