// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ PublishSubscribe Model,Provider!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();
// 创建一个fanout交换机
channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);
// 创建几个队列
channel.QueueDeclare(queue: "fanout_queue1", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "fanout_queue2", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "fanout_queue3", durable: false, exclusive: false, autoDelete: false, arguments: null);
// 绑定队列到交换机
channel.QueueBind(queue: "fanout_queue1", exchange: "fanout_exchange", routingKey: "");
channel.QueueBind(queue: "fanout_queue2", exchange: "fanout_exchange", routingKey: "");
channel.QueueBind(queue: "fanout_queue3", exchange: "fanout_exchange", routingKey: "");
// 发送一些消息到队列中
const int max = 30;
int i = 0;
while (true && i < max)
{
    var message = $"Hello PublishSubscribe-{i}";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "fanout_exchange", routingKey: "", basicProperties: null, body: body);
    Console.WriteLine($" [x] Sent {message}");
    i++;
    Thread.Sleep(1000);
}