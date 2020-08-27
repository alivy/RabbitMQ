using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQSend
{
    class Program
    {
        /// <summary>
        /// 发送MQ消息
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string exchangeName = "mytopicerexchange";//"logExchange";
            string exchangeType = ExchangeType.Direct;
            string queueName = "logElse";//"myfanoutexqueue";

            string routingKey = string.Empty;

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "192.168.1.94"; //"127.0.0.1"; //
            factory.UserName = "yhm"; //"yanhaomiao"; //
            factory.Password = "123456";




            //factory.VirtualHost = "lolocalhost:15672";
            //第一步：创建Connection 
            using (var connection = factory.CreateConnection())
            //第二步：创建channel
            using (var channel = connection.CreateModel())
            {
                //第三步： 定义交换机exchanges 可使用默认
                //channel.ExchangeDeclare(
                //    exchange: exchangeName,
                //    type: exchangeType);

                ////一定要绑定至消息队列
                //channel.QueueBind(queue: queueName,
                //    exchange: exchangeName,
                //    routingKey: routingKey,
                //    arguments: null);

                ////第四部： 定义队列
                //channel.QueueDeclare(queue: queueName,
                //     durable: true, //耐久的，队列不会因为RabbitMQ启动而消失
                //     exclusive: false,
                //     autoDelete: false,
                //     arguments: null);

                // var basicProperties = channel.CreateBasicProperties();
                //haead满足其一就可以获取
                //basicProperties.Headers = new Dictionary<string, object>() { { "x-math", "any" }, { "userName", "jack" }, { "password", "123456" } };
                for (int i = 0; i < 100; i++)
                {
                    //第五步：发布消息
                    string message = $"传入第{i}个消息"; //传递的消息内容
                    //topic接口可使用
                    routingKey = i % 3 == 0 ? ".com" :"abc.cn";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: exchangeName,
                                routingKey: routingKey,
                                basicProperties: null,// basicProperties,
                                body: body);
                }
            }
        }



    }
}
