using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.Common;
using System.Collections.Concurrent;
using System.Text;

namespace Rpc.Client;
public class RpcClient
{
    private const string QUEUE_NAME = "rpc_queue";

    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new();

    public RpcClient()
    {

        connection = RabbitHelper.GetFactory().CreateConnection();
        channel = connection.CreateModel();
        // declare a server-named queue
        replyQueueName = channel.QueueDeclare(queue: "").QueueName;
        consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs)) return;
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            _ = tcs.TrySetResult(response);
        };

        _ = channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);
    }

    public Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
    {
        IBasicProperties props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var tcs = new TaskCompletionSource<string>();
        _ = callbackMapper.TryAdd(correlationId, tcs);

        channel.BasicPublish(exchange: "", routingKey: QUEUE_NAME, basicProperties: props, body: messageBytes);

        _ = cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out var tmp));
        return tcs.Task;
    }

    public void Close() => connection.Close();
}