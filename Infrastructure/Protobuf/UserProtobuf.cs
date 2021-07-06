using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Protobuf
{
    [ProtoContract]
    public class UserProtobuf
    {
        [ProtoMember(1)]
        public long Id { get; set; }
        [ProtoMember(2)]
        public string Fullname { get; set; }
        [ProtoMember(3)]
        public string Email { get; set; }
    }
}
