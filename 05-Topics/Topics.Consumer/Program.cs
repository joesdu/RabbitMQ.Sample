// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ Topics Model,Consumer!");

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

var consumer = new EventingBasicConsumer(channel);
// 加个随机数让消费者慢慢消费.
var rand = new Random();
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Consumer Received {message}-{ea.RoutingKey}");
    var second = rand.Next(1, 2);
    Thread.Sleep(second * 1000);
    // 确认消费,批量签收可以降低资源消耗.
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: true);
};
// 消费队列里的消息.
channel.BasicConsume(queue: "topic_queue1", autoAck: false, consumer: consumer);
channel.BasicConsume(queue: "topic_queue2", autoAck: false, consumer: consumer);
channel.BasicConsume(queue: "topic_queue3", autoAck: false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();