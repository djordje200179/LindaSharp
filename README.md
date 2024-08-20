# LindaSharp
LindaSharp is an implementation of the tuple space using C#.

The library is divided into multiple parts:
- Local: A tuple space that can be used in a single process.
- Server: A tuple space that can be used in a distributed environment.
- Clients: Clients that can connect to a server and interact with the tuple space.
    - .NET
    - Python
    - Java
    - JavaScript *- Coming soon*
    - Rust *- Coming soon*

Supported operations:
- `out`: Insert a tuple to the tuple space
- `in`: Take a tuple from the tuple space
- `rd`: Read a tuple from the tuple space (without removing it)
- `inp`: Try to take a tuple from the tuple space immediately
- `rdp`: Try to read a tuple from the tuple space immediately (without removing it)
- `eval` - Run task in the tuple space
    - Local tuple space will run the function in a separate thread
    - Server tuple space will run the script on the server side
    - Clients will send the script to the server to be executed

## Server
Server is implemented using ASP.NET Core and supports:
- REST API
- gRPC **- mainly used**
- GUI dashboard

### Scripting
Server supports running scripts that will be executed on the server side.
Currently, only IronPython (.NET implementation of Python) is supported, 
but more languages will be added in the future.

Scripts will be executed in a separate thread and will have access to the tuple space
through the `linda` object.
Also, registered scripts can be invoked with a parameter, which can be accessed
through the `param` variable.

Script execution status can be checked with an execution ID, which is returned
when the script is started.

Example of an Ironpython script:
```python
while True:
	if linda.try_query("done"):
		break
	
	_, a = linda.get("a", None)
	_, b = linda.get("b", None)
	_, ind = linda.get("ind", None)

	ind += 1
	c = a + b

	linda.put("fib", ind, c)

	linda.put("a", b)
	linda.put("b", c)
	linda.put("ind", ind)
```

## Clients
Clients are written in way that is idiomatic to the language they are written in.
Most of the clients are written in a way that can be used in an asynchronous environment.