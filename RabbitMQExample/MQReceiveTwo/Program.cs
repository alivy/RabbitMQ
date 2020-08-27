using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQReceiveTwo
{
    class Program
    {

        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.1.94"; //"127.0.0.1"; //
            factory.UserName = "yhm"; //"yanhaomiao"; //
            factory.Password = "123456";
            factory.Port = AmqpTcpEndpoint.UseDefaultPort;
            factory.VirtualHost = "/";


            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    MyTopicExchange2(channel);
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// 扇出交换机
        /// 
        /// </summary>
        private static void MyFanoutExchange(IModel channel)
        {
            string exchangeName = "myfanoutexchange";
            string exchangeType = ExchangeType.Fanout;
            string queueName = "myfanoutexqueue2";
            string routingKey = string.Empty;  //
            channel.ExchangeDeclare("myfanoutexchange", ExchangeType.Fanout, true, false, null);
            var temporaryQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: temporaryQueueName, exchange: "myfanoutexchange", routingKey: string.Empty, arguments: null); //一定要绑定至消息队列
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var routingKeyResult = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKeyResult, message);
            };
            channel.BasicConsume(queue: temporaryQueueName, autoAck: true, consumer: consumer);
            Console.WriteLine("consumer1 的myfanoutexchange启动完毕");
        }

        /// <summary>
        /// Topic类型
        /// 可根据消息内容进行队列匹配
        /// </summary>
        private static void MyTopicExchange(IModel channel)
        {
            channel.ExchangeDeclare("mytopicerexchange", ExchangeType.Topic, true, false, null);
            var temporaryQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: temporaryQueueName, exchange: "mytopicerexchange", routingKey: "*.cn", arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };
            channel.BasicConsume(queue: temporaryQueueName, autoAck: true, consumer: consumer);
            Console.WriteLine("MQReceiveTwo_consumer的MyTopicExchange2启动完毕");
        }

        /// <summary>
        /// Topic类型
        /// 可根据消息内容进行队列匹配
        /// </summary>
        private static void MyTopicExchange2(IModel channel)
        {
            channel.ExchangeDeclare("mytopicerexchange", ExchangeType.Topic, true, false, null);
            var temporaryQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: temporaryQueueName, exchange: "mytopicerexchange", routingKey: "abc.*", arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };
            channel.BasicConsume(queue: temporaryQueueName, autoAck: true, consumer: consumer);
            Console.WriteLine("MQReceiveTwo_consumer2的MyTopicExchange2启动完毕");
        }
    }
}
