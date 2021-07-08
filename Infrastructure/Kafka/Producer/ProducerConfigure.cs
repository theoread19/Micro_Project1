using Confluent.Kafka;
using Google.Protobuf;
using System;
using UserProtoBufService;

namespace Infrastructure.Kafka.Producer
{
    public class ProducerConfigure
    {

        private readonly ProducerConfig config = new ProducerConfig{ BootstrapServers = "localhost:9092" };
        private readonly string _topic;

        public ProducerConfigure(string topic)
        {
            this._topic = topic;
        }

        public Object SendToKafka(UserProtoReq data, string key)
        {
            using (var producer =
                 new ProducerBuilder<string, UserProtoReq>(config)
                 .SetValueSerializer(new UserSerializer())// ProtobufSerializer
                 .Build())
            {
                try
                {
                        return producer.ProduceAsync(this._topic, new Message<string, UserProtoReq> {Key = key, Value = data })
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Oops, something went wrong: {e}");
                }
                finally
                {
                    producer.Flush();
                }
            }
            return null;
        }
    }
}
