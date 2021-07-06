using Confluent.Kafka;
using Infrastructure.Protobuf;
using System;

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

        public Object SendToKafka(byte[] data)
        {
            using (var producer =   
                 new ProducerBuilder<string, byte[]>(config).Build())
            {
                try
                {
                        return producer.ProduceAsync(this._topic, new Message<string, byte[]> {Key = "message", Value = data })
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Oops, something went wrong: {e}");
                }
            }
            return null;

        }
    }
}
