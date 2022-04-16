// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ Topics Model,Producer!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();
// 创建一个fanout交换机
channel.ExchangeDeclare(exchange: "topic_exchange", type: ExchangeType.Topic);
// 创建几个队列
channel.QueueDeclare(queue: "topic_queue1", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "topic_queue2", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.QueueDeclare(queue: "topic_queue3", durable: false, exclusive: false, autoDelete: false, arguments: null);
// 绑定队列到交换机
channel.QueueBind(queue: "topic_queue1", exchange: "topic_exchange", routingKey: "i.data.*");
channel.QueueBind(queue: "topic_queue2", exchange: "topic_exchange", routingKey: "i.data.456");
channel.QueueBind(queue: "topic_queue3", exchange: "topic_exchange", routingKey: "i.data.789");
// 发送一些消息到队列中
const int max = 30;
int i = 0;
while (true && i < max)
{
    var message = $"Hello Topic-{i}";
    var body = Encoding.UTF8.GetBytes(message);
    switch (i % 3)
    {
        case 0:
            channel.BasicPublish(exchange: "topic_exchange", routingKey: "i.data.123", basicProperties: null, body: body);
            break;
        case 1:
            channel.BasicPublish(exchange: "topic_exchange", routingKey: "i.data.456", basicProperties: null, body: body);
            break;
        case 2:
            channel.BasicPublish(exchange: "topic_exchange", routingKey: "i.data.789", basicProperties: null, body: body);
            break;
    }
    Console.WriteLine($" [x] Sent {message}");
    i++;
    Thread.Sleep(1000);
}