using LindaSharp.Client;

var remoteClient = new RemoteLinda("localhost", 8080);

remoteClient.Out(new object[] { "mutex" });
remoteClient.Out(new object[] { "iteration", 1 });
remoteClient.Out(new object[] { "a", 0 });
remoteClient.Out(new object[] { "b", 1 });

remoteClient.Eval(linda => {
	linda.Out(new object[] { "num" });
});