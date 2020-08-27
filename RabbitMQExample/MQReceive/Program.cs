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
            factory.HostName = "192.168.1.94"; //"127.0.0.1"; //
            factory.UserName = "yhm"; //"yanhaomiao"; //
            factory.Password = "123456";
            factory.Port = AmqpTcpEndpoint.UseDefaultPort;
            factory.VirtualHost = "/";
            //factory.Protocol = Protocols.DefaultProtocol;
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    #region 获取队列方式
                    //LogConsumer(channel);
                    //LogError(channel);
                    //MyFanoutExchange(channel);
                    //MyHeaderExchange(channel);
                    MyTopicExchange(channel);
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

        /// <summary>
        /// 扇出交换机
        /// 
        /// </summary>
        private static void MyFanoutExchange(IModel channel)
        {

            channel.ExchangeDeclare("myfanoutexchange", ExchangeType.Fanout, true, false, null);
            var temporaryQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: temporaryQueueName, exchange: "myfanoutexchange", routingKey: string.Empty, arguments: null); //一定要绑定至消息队列
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };
            channel.BasicConsume(queue: temporaryQueueName, autoAck: true, consumer: consumer);
            Console.WriteLine("consumer的myfanoutexchange启动完毕");
        }




        /// <summary>
        /// Header类型
        /// 
        /// </summary>
        private static void MyHeaderExchange(IModel channel)
        {
            channel.ExchangeDeclare("myheaderexchange", ExchangeType.Headers, true, false, null);
            var temporaryQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: temporaryQueueName, exchange: "myheaderexchange", routingKey: string.Empty, arguments: new Dictionary<string, object>()
            {
                {"x-math","any" }, {"userName","jack" }, {"password","123456" }
            }); //一定要绑定至消息队列 如 "x-math","any"时表示头部匹配一个活多个即可 ，"x-math","all"时表示头部必须全部满足
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };
            channel.BasicConsume(queue: temporaryQueueName, autoAck: true, consumer: consumer);
            Console.WriteLine("consumer的MyHeaderExchange启动完毕");
        }


        /// <summary>
        /// Topic类型
        /// 可根据消息内容进行队列匹配
        /// </summary>
        private static void MyTopicExchange(IModel channel)
        {
            channel.ExchangeDeclare("mytopicerexchange", ExchangeType.Topic, true, false, null);
            var temporaryQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: temporaryQueueName, exchange: "mytopicerexchange", routingKey: "*.com", arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };
            channel.BasicConsume(queue: temporaryQueueName, autoAck: true, consumer: consumer);
            Console.WriteLine("consumer的MyTopicExchange启动完毕");
        }


    }

}
