using LindaSharp;
using LindaSharp.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddSingleton(new SharedLinda(new LocalLinda()))
	.AddControllers()
	.AddJsonOptions(options => {
		options.JsonSerializerOptions.Converters.Add(new TupleJsonDeserializer());
		options.JsonSerializerOptions.WriteIndented = true;
	});

var app = builder.Build();

app.MapControllers();

app.Run();