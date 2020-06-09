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

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            factory.UserName = "yanhaomiao";
            factory.Password = "123456";
            //factory.VirtualHost = "lolocalhost:15672";
            //第一步：创建Connection 
            using (var connection = factory.CreateConnection())
            //第二步：创建channel
            using (var channel = connection.CreateModel())
            {
                //第三步： 定义交换机exchanges 可使用默认
                channel.ExchangeDeclare("myexchange", ExchangeType.Direct, true, false, null);
                channel.QueueBind("mytestQueue", "myexchange", "mytestQueue", null); //一定要绑定至消息队列
                
                //第四部： 定义队列
                channel.QueueDeclare(queue: "mytestQueue",
                     durable: true, //耐久的，队列不会因为RabbitMQ启动而消失
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
                for (int i = 0; i < 100; i++)
                {
                    //第五步：发布消息
                    string message = $"传入第{i}个消息"; //传递的消息内容
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(string.Empty, "mytestQueue", null, body);
                }
            }
        }
    }
}
