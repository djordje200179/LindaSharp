using LindaSharp.Client;

var remoteClient = new RemoteLinda("localhost", 8080);

remoteClient.Out(new object[] { "mutex" });