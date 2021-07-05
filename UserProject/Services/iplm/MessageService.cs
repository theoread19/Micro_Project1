using Infrastructure.APICall;
using Infrastructure.Kafka.Producer;
using Infrastructure.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Protobuf;
using UserProject.DTOs.Request;

namespace UserProject.Services.iplm
{
    public class MessageService : IMessageService
    {

        private ProducerConfigure configure = new ProducerConfigure("test");

        public List<MessageRequest> GetMessageBySenderId(long id)
        {

            var responseString = ApiCall.GetApi("https://localhost:44359/api/Message/senderId=" + id);
            JArray jsonResponse = JArray.Parse(responseString);
            UserProtobuf temp = new UserProtobuf
            {
                id = 1L,
                Fullname = "a",
                Email = "@"
            };

            configure.SendToKafka(Serialize.ProtoSerialize<UserProtobuf>(temp));
            return jsonResponse.ToObject<List<MessageRequest>>();
        }

        public void ModifiyMessage(MessageRequest req)
        {
            var json = JsonConvert.SerializeObject(req);
            ApiCall.PutApi("https://localhost:44359/api/Message/", json);
        }

        public void RemoveMessage(long id)
        {
            ApiCall.DeleteApi("https://localhost:44359/api/Message/" + id, id);
        }

        public void SendMessage(MessageRequest req)
        {
            var json = JsonConvert.SerializeObject(req);
            ApiCall.PostApi("https://localhost:44359/api/Message", json);
        }


    }
}
