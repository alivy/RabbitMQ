using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQSendTwo
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            factory.UserName = "yanhaomiao";
            factory.Password = "123456";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                LogSend(channel);
            }
        }

        /// <summary>
        /// 简单消息发送
        /// </summary>
        /// <param name="model"></param>
        private static void MsgSend(IModel channel)
        {
            for (int i = 0; i < 100; i++)
            {
                //第五步：发布消息
                string message = $"传入第{i}个消息"; //传递的消息内容
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(string.Empty, "mytestQueue", null, body);
            }
        }


        /// <summary>
        /// 日志消息发送
        /// </summary>
        private static  void LogSend(IModel channel)
        {
            var arryLog = new string[] { "debug", "info", "warning", "error" };

            for (int i = 0; i < 100; i++)
            {
                var num = i % arryLog.Length;
                //第五步：发布消息
                string message = $"{arryLog[num]}消息:传入第{i}个消息,"; //传递的消息内容
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "logExchange", routingKey: arryLog[num], basicProperties: null, body: body);
            }
        }
    }
}
