// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ PublishSubscribe Model,Consumer!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();

# region 消费者中写这段内容是为了防止消费者先启动,造成异常
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
# endregion

var consumer = new EventingBasicConsumer(channel);
// 加个随机数让消费者慢慢消费.
var rand = new Random();
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Consumer Received {message}");
    var second = rand.Next(1, 2);
    Thread.Sleep(second * 1000);
    // 确认消费
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);    
};
// 消费队列里的消息.
channel.BasicConsume(queue: "fanout_queue1", autoAck: false, consumer: consumer);
channel.BasicConsume(queue: "fanout_queue2", autoAck: false, consumer: consumer);
channel.BasicConsume(queue: "fanout_queue3", autoAck: false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();