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
                    EventingBasicConsumer(channel);
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
    }
}
