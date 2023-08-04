using LindaSharp;
using LindaSharp.Server;
using JSON = Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JSON.JsonOptions>(options => {
	options.SerializerOptions.Converters.Add(new TupleJsonDeserializer());
	options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

var linda = new LocalLinda();

app.MapGet("/ping", () => "pong");

app.MapPost("/out", ([FromBody] object[] tuple) => {
	try {
		linda.Out(tuple);
	} catch (ObjectDisposedException) {
		return Results.StatusCode((int)HttpStatusCode.InternalServerError);
	}

	return Results.StatusCode((int)HttpStatusCode.Created);
});

app.MapGet("/in", ([FromBody] object?[] tuplePattern) => {
	try {
		var tuple = linda.In(tuplePattern);

		return Results.Json(tuple);
	} catch (ObjectDisposedException) {
		return Results.StatusCode((int)HttpStatusCode.InternalServerError);
	}
});

app.MapGet("/rd", ([FromBody] object?[] tuplePattern) => {
	try {
		var tuple = linda.Rd(tuplePattern);

		return Results.Json(tuple);
	} catch (ObjectDisposedException) {
		return Results.StatusCode((int)HttpStatusCode.InternalServerError);
	}
});

app.MapGet("/inp", ([FromBody] object?[] tuplePattern) => {
	try {
		var status = linda.Inp(tuplePattern, out var tuple);

		return status ? Results.Json(tuple) : Results.NotFound();
	} catch (ObjectDisposedException) {
		return Results.StatusCode((int)HttpStatusCode.InternalServerError);
	}
});

app.MapGet("/rdp", ([FromBody] object?[] tuplePattern) => {
	try {
		var status = linda.Rdp(tuplePattern, out var tuple);

		return status ? Results.Json(tuple) : Results.NotFound();
	} catch (ObjectDisposedException) {
		return Results.StatusCode((int)HttpStatusCode.InternalServerError);
	}
});

app.MapPost("/eval", (HttpRequest request) => {
	StreamReader reader;
	if (request.ContentType == "text/ironpython")
		reader = new StreamReader(request.Body);
	else if (request.Form.Files.Count == 1) {
		var file = request.Form.Files[0];
		reader = new StreamReader(file.OpenReadStream());
	} else
		return Results.StatusCode((int)HttpStatusCode.UnsupportedMediaType);

	var codeReadingTask = reader.ReadToEndAsync();
	codeReadingTask.Wait();
	var pythonCode = codeReadingTask.Result;

	linda.Eval(pythonCode);

	return Results.Ok();
});

app.MapPut("/eval/{key}", (HttpRequest request, string key) => {
	StreamReader reader;
	if (request.ContentType == "text/ironpython")
		reader = new StreamReader(request.Body);
	else if (request.Form.Files.Count == 1) {
		var file = request.Form.Files[0];
		reader = new StreamReader(file.OpenReadStream());
	} else
		return Results.StatusCode((int)HttpStatusCode.UnsupportedMediaType);

	var codeReadingTask = reader.ReadToEndAsync();
	codeReadingTask.Wait();
	var pythonCode = codeReadingTask.Result;

	linda.EvalRegister(key, pythonCode);

	return Results.Ok();
});

app.MapPost("/eval/{key}", (HttpRequest request, string key, [FromBody] object? parameter) => {
	try {
		linda.EvalInvoke(key, parameter);
	} catch (KeyNotFoundException) {
		return Results.NotFound();
	}

	return Results.Ok();
});

app.Run();