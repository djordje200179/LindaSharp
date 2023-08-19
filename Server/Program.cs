using LindaSharp;
using LindaSharp.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddSingleton(new SharedLinda(new LocalLinda()))
	.AddControllersWithViews()
	.AddJsonOptions(options => {
		options.JsonSerializerOptions.Converters.Add(new TupleJsonSerializer());
		options.JsonSerializerOptions.WriteIndented = true;
	});

var app = builder.Build();

app.MapControllers();

app.Run();