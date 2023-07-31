using LindaSharp;
using LindaSharp.Server;
using JSON = Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JSON.JsonOptions>(options => {
	options.SerializerOptions.Converters.Add(new TupleJsonDeserializer());
	options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

var linda = new LocalLinda();

app.MapGet("/ping", () => "pong");

app.MapPost("/out", ([FromBody] object[] tuple) => {
	linda.Out(tuple);

	return "OK";
});

app.MapGet("/in", ([FromBody] object?[] tuplePattern) => {
	return linda.In(tuplePattern);
});

app.MapGet("/rd", ([FromBody] object?[] tuplePattern) => {
	return linda.Rd(tuplePattern);
});

app.MapGet("/inp", ([FromBody] object?[] tuplePattern) => {
	return linda.Inp(tuplePattern, out var tuple) ? tuple : null;
});

app.MapGet("/rdp", ([FromBody] object?[] tuplePattern) => {
	return linda.Rdp(tuplePattern, out var tuple) ? tuple : null;
});

app.Run();