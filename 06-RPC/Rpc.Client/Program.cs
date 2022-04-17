// See https://aka.ms/new-console-template for more information
using Rpc.Client;

Console.WriteLine("RabbitMQ RPC Client");
string n = args.Length > 0 ? args[0] : "30";
Task t = InvokeAsync(n);
t.Wait();

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static async Task InvokeAsync(string n)
{
    var rpcClient = new RpcClient();

    Console.WriteLine(" [x] Requesting fib({0})", n);
    var response = await rpcClient.CallAsync(n.ToString());
    Console.WriteLine(" [.] Got '{0}'", response);

    rpcClient.Close();
}