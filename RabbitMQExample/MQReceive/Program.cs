using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQReceive
{
    class Program
    {
        /// <summary>
        /// 接收MQ消息处理
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            factory.UserName = "yanhaomiao";
            factory.Password = "123456";
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    #region 获取队列方式
                    LogConsumer(channel);
                    LogError(channel);
                    #endregion
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// BasicGet主动拉取
        /// </summary>
        private static void BasicGet(IModel channel)
        {
            //获取队列
            var result = channel.BasicGet("mytestQueue", true);
            var message = Encoding.UTF8.GetString(result.Body.ToArray());
            Console.WriteLine("已接收： {0}", message);
        }

        /// <summary>
        /// 订阅式轮询接收
        /// </summary>
        private static void EventingBasicConsumer(IModel channel)
        {
            channel.QueueDeclare("mytestQueue", true, false, false, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("已接收： {0}", message);
                //int dots = message.Split('.').Length - 1;
                //Thread.Sleep(dots * 1000);
                //Console.WriteLine(" [x] Done");
            };
            channel.BasicConsume(queue: "mytestQueue", autoAck: true, consumer: consumer);
        }



        /// <summary>
        ///  日志记录场景分析之如何使用将routingkey绑定到各自的queue来实现实际场景的按需定制
        /// </summary>
        private static void LogConsumer(IModel channel)
        {
            channel.ExchangeDeclare("logExchange", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare("logElse", true, false, false, null);
            var arryLog = new string[3] { "debug", "info", "warning" };
            for (int i = 0; i < arryLog.Length; i++)
            {
                channel.QueueBind("logElse", "logExchange", arryLog[i], null); //一定要绑定至消息队列
            }
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine("已接收： {0}", message);
            };
            channel.BasicConsume(queue: "logElse", autoAck: true, consumer: consumer);
        }


        /// <summary>
        ///  记录错误日志
        /// </summary>
        private static void LogError(IModel channel)
        {
            channel.ExchangeDeclare("logExchange", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare("logError", true, false, false, null);
          
            channel.QueueBind("logError", "logExchange", "error", null); //一定要绑定至消息队列
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine("已接收： {0}", message);
            };
            channel.BasicConsume(queue: "logError", autoAck: true, consumer: consumer);
        }

    }
}
