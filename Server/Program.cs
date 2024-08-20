using LindaSharp;
using LindaSharp.ScriptEngine;
using LindaSharp.Server;
using LindaSharp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

var localLinda = new LocalLinda();
var scriptEvalLinda = new ScriptEvalLinda(localLinda);

builder.Services.AddGrpc();

builder.Services
	.AddSingleton<ILinda>(localLinda)
	.AddSingleton<ISpaceViewLinda>(localLinda)
	.AddSingleton<IScriptEvalLinda>(scriptEvalLinda);

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
app.MapGrpcService<HealthService>();
app.MapGrpcService<ScriptsService>();

app.MapControllers();

if (app.Environment.IsDevelopment()) {
	app.UseDeveloperExceptionPage();
	app.UseSwagger().UseSwaggerUI();
}

app.Run();