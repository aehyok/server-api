using Lychee.Core.Infrastructures;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DVS.Common.MQ
{
    /// <summary>
    /// 订阅的消费者都会接收到消息
    /// </summary>
    public static class RabbitMQSubscriber
    {
        private static IModel channel;
        private static IConnection connection;

        public static IApplicationBuilder UseRabbitMQ(this IApplicationBuilder app, Action<RabbitMQOptions> config)
        {
            try
            {
                RabbitMQOptions options = new RabbitMQOptions();
                config(options);
                var factory = new ConnectionFactory() { VirtualHost = options.VirtualHost, HostName = options.Host, Port = options.Port, Password = options.Password, UserName = options.UserName };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName,
                                  exchange: "SSO_USER_LOGOUT",
                                  routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
              {
                  var body = ea.Body.ToArray();
                  var message = Encoding.UTF8.GetString(body);
                  // 清理token信息
                  LogoutResult logoutInfo = JsonSerializer.Deserialize<LogoutResult>(message, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                  foreach (var token in logoutInfo.Value)
                  {
                      string tokenKey = Utils.MD5(token);
                      RedisHelper.Del("sso." + tokenKey);
                  }
              };
                channel.BasicConsume(queue: "",
                                     autoAck: true,
                                     consumer: consumer);

            }
            catch (Exception ex)
            {

            }
            return app;
        }
    }

    public class LogoutResult
    {
        public string Action { get; set; }
        public string Msg { get; set; }
        public List<string> Value { get; set; }
    }

    public class RabbitMQOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
    }
}
