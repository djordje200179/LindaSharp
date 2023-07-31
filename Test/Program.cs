using LindaSharp.Client;

var remoteClient = new RemoteLinda("localhost", 8080);

remoteClient.Out(new object[] { "num", 1 });

var tuple = remoteClient.In(new object?[] { "num", null });
Console.WriteLine(tuple[1]);