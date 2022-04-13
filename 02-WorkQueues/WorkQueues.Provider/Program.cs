// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ WorkQueues Model,Provider!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "work_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

var properties = channel.CreateBasicProperties();
properties.Persistent = true;

const int max = 30;
int i = 0;
while (true && i < max)
{
    var message = $"Hello WorkQueues-{i}";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "", routingKey: "work_queue", basicProperties: properties, body: body);
    Console.WriteLine($" [x] Sent {message}");
    i++;
    Thread.Sleep(1000);
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
