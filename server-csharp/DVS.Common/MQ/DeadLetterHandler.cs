using DVS.Models.Models.SunFile;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DVS.Common.MQ
{
    public static class DeadLetterHandler
    {
        public static void SendDelayMessage(this FileCleanTask cleanTask, Action<RabbitMQOptions> configure)
        {
            RabbitMQOptions rabbitMQOptions = new RabbitMQOptions();
            configure(rabbitMQOptions);
            var factory = new ConnectionFactory() { HostName = rabbitMQOptions.Host, Port = rabbitMQOptions.Port, VirtualHost = rabbitMQOptions.VirtualHost, Password = rabbitMQOptions.Password, UserName = rabbitMQOptions.UserName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    // 死信交换机
                    string deadEx = "FileCleanTaskTimeoutEx";
                    // 文件删除交换器
                    string delayEx = "FileCleanTaskEx";
                    // 死信队列
                    string deadQueueName = "FileCleanTaskTimeoutQueue";
                    // 文件删除队列
                    string queueName = "FileCleanTaskQueue";
                    string routingKey = "FileCleanTask";


                    // 创建死信队列
                    channel.ExchangeDeclare(deadEx, type: ExchangeType.Fanout, durable: true, autoDelete: false);
                    channel.QueueDeclare(deadQueueName, durable: true, exclusive: false, autoDelete: false);
                    channel.QueueBind(deadQueueName, deadEx, routingKey);

                    // 创建延时队列
                    channel.QueueDeclare(queueName, true, false, false, new Dictionary<string, object>(){
                        { "x-dead-letter-exchange",deadEx },
                        { "x-dead-letter-routing-key",routingKey },
                        { "x-message-ttl",10000}
                    });
                    channel.QueueBind(queueName, exchange: delayEx, routingKey, null);

                    // 发送数据到延迟队列
                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cleanTask));
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    channel.BasicPublish(exchange: delayEx, routingKey, properties, body);
                }
            }
        }
    }
}
