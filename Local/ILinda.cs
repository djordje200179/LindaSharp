﻿using System.Diagnostics.CodeAnalysis;

namespace LindaSharp;

public interface ILinda : IDisposable {
	public void Out(object[] tuple);

	public object[] In(object?[] tuplePattern);
	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);

	public object[] Rd(object?[] tuplePattern);
	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);

	public void Eval(string pythonCode);
	public void EvalFile(string pythonCodePath);
}