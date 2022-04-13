// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Common;
using System.Text;

// 该启动模式请从编译后的exe文件中启动,可启动多个实例来体现多消费者的情况.

Console.WriteLine("RabbitMQ WorkQueues Model,Consumer!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();

_ = channel.QueueDeclare(queue: "work_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new EventingBasicConsumer(channel);
var rand = new Random();
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Consumer Received {message}");
    var second = rand.Next(1, 3);
    Thread.Sleep(second * 1000);
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
_ = channel.BasicConsume(queue: "work_queue", autoAck: false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();