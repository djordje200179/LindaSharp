using LindaSharp;
using LindaSharp.Server;
using LindaSharp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services
	.AddSingleton(new SharedLinda(new LocalLinda()));

builder.Services
	.AddControllersWithViews()
	.AddJsonOptions(options => {
		options.JsonSerializerOptions.Converters.Add(new TupleJsonSerializer());
		options.JsonSerializerOptions.WriteIndented = true;
	});

builder.Services
	.AddEndpointsApiExplorer()
	.AddSwaggerGen();

var app = builder.Build();

// TODO: Change endpoint
app.MapGrpcService<ActionsService>();

app.MapControllers();

if (app.Environment.IsDevelopment()) {
	app.UseDeveloperExceptionPage();
	app.UseSwagger().UseSwaggerUI();
}

app.Run();