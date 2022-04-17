﻿// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Common;
using System.Text;

Console.WriteLine("RabbitMQ RPC Server!");

using var connection = RabbitHelper.GetFactory().CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
Console.WriteLine(" [x] Awaiting RPC requests");

consumer.Received += (model, ea) =>
{
    string response = "";

    var body = ea.Body.ToArray();
    var props = ea.BasicProperties;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = props.CorrelationId;

    try
    {
        var message = Encoding.UTF8.GetString(body);
        int n = int.Parse(message);
        Console.WriteLine(" [.] fib({0})", message);
        response = fib(n).ToString();
    }
    catch (Exception e)
    {
        Console.WriteLine(" [.] " + e.Message);
        response = "";
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
};

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

/// <summary>
/// Assumes only valid positive integer input.
/// Don't expect this one to work for big numbers, and it's probably the slowest recursive implementation possible.
/// </summary>
static int fib(int n)
{
    return n == 0 || n == 1 ? n : fib(n - 1) + fib(n - 2);
}