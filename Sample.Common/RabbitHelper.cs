using RabbitMQ.Client;

namespace Sample.Common;
public static class RabbitHelper
{
    public static ConnectionFactory GetFactory() => new()
    {
        HostName = "192.168.2.27",
        Port = 5672,
        UserName = "admin",
        Password = "&duyu789",
        VirtualHost = "/"
    };
}
