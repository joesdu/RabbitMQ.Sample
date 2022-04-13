// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ HelloWorld Model,Provider!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "helloworld", durable: false, exclusive: false, autoDelete: false, arguments: null);

const int max = 30;
int i = 0;
while (true && i < max)
{
    var message = $"Hello World!{i}";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "", routingKey: "helloworld", basicProperties: null, body: body);
    Console.WriteLine($" [x] Sent {message}");
    i++;
    Thread.Sleep(1000);
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
