using DVS.Common.FFMpeg;
using DVS.Models.Models.MediaTransform;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DVS.Common.MQ
{
    /// <summary>
    /// 每个消息只被一个消费者消费
    /// </summary>
    public static class RabbitMQWorker
    {

        private static IModel channel;
        private static IConnection connection;
 
    
        public static IApplicationBuilder UseRabbitMQForMidiaTransform(this IApplicationBuilder app, IHttpClientFactory httpFactory, Action<RabbitMQOptions> config,IConfiguration configuration)
        {
 
            RabbitMQOptions rabbitMQOptions = new RabbitMQOptions();
            config(rabbitMQOptions);
            var factory = new ConnectionFactory() { HostName = rabbitMQOptions.Host, Port = rabbitMQOptions.Port, VirtualHost = rabbitMQOptions.VirtualHost, Password = rabbitMQOptions.Password, UserName = rabbitMQOptions.UserName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "MediaTransformQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine("会话创建完成，等待转码任务");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("收到转码信息:" + message);
                TransformTask transformTask = JsonSerializer.Deserialize<TransformTask>(message, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                VideoTransform videoTransform = new VideoTransform(httpFactory,configuration);
                // 生成缩略图
                _ = videoTransform.GetVideoThumbnailAsync(transformTask);
                // 转换h264

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "MediaTransformQueue",
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            return app;
        }

        public static void SendTask(this TransformTask task, Action<RabbitMQOptions> config)
        {
            RabbitMQOptions rabbitMQOptions = new RabbitMQOptions();
            config(rabbitMQOptions);
            var factory = new ConnectionFactory() { HostName = rabbitMQOptions.Host, Port = rabbitMQOptions.Port, VirtualHost = rabbitMQOptions.VirtualHost, Password = rabbitMQOptions.Password, UserName = rabbitMQOptions.UserName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "MediaTransformQueue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);


                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(task));

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "MediaTransformQueue",
                                         basicProperties: properties,
                                         body: body);
                }
            }
        }
    }

}
