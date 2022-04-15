// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ Routing Model,Provider!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();
// 创建一个fanout交换机
channel.ExchangeDeclare(exchange: "direct_exchange", type: ExchangeType.Direct);
// 创建几个队列
channel.QueueDeclare(queue: "direct_red", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "direct_green", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "direct_yellow", durable: false, exclusive: false, autoDelete: false, arguments: null);
// 绑定队列到交换机
channel.QueueBind(queue: "direct_red", exchange: "direct_exchange", routingKey: "red");
channel.QueueBind(queue: "direct_green", exchange: "direct_exchange", routingKey: "green");
channel.QueueBind(queue: "direct_yellow", exchange: "direct_exchange", routingKey: "yellow");
// 发送一些消息到队列中
const int max = 30;
int i = 0;
while (true && i < max)
{
    var message = $"Hello Routing-{i}";
    var body = Encoding.UTF8.GetBytes(message);
    switch (i % 3)
    {
        case 0:
            channel.BasicPublish(exchange: "direct_exchange", routingKey: "red", basicProperties: null, body: body);
            break;
        case 1:
            channel.BasicPublish(exchange: "direct_exchange", routingKey: "green", basicProperties: null, body: body);
            break;
        case 2:
            channel.BasicPublish(exchange: "direct_exchange", routingKey: "yellow", basicProperties: null, body: body);
            break;
    }
    Console.WriteLine($" [x] Sent {message}");
    i++;
    Thread.Sleep(1000);
}