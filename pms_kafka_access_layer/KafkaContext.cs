using ProductManagementSystem.Entities.Models;
using System.Net;
using Confluent.Kafka;
using System.Text.Json;
using ProductManagementSystem.Contracts;

namespace ProductManagementSystem.KafkaAccessLayer
{
    public class KafkaContext<T>: IKafkaContext<T>
    {
        public ProducerConfig KafkaConfig { get; set; }

        public KafkaContext(string bootstrapServers)
        {
            KafkaConfig = new ProducerConfig { BootstrapServers = bootstrapServers };
        }

        public void SendProductToKafkaTopic(string kafkaTopic, T entity)
        {
            Action<DeliveryReport<Null, string>> handler = r => Console.WriteLine(!r.Error.IsError ? $"Delivered message to {r.TopicPartitionOffset}" : $"Delivery Error: {r.Error.Reason}");
            using (var producer = new ProducerBuilder<Null, string>(KafkaConfig).Build())
            {
                producer.Produce(kafkaTopic, new Message<Null, string> { Value = JsonSerializer.Serialize(entity) }, handler);

                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}