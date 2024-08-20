namespace LindaSharp;

public interface ILinda {
	public Task Put(params object[] tuple);

	public Task<object[]> Get(params object?[] pattern);
	public Task<object[]> Query(params object?[] pattern);

	public Task<object[]?> TryGet(params object?[] pattern);
	public Task<object[]?> TryQuery(params object?[] pattern);
}

public interface ISpaceViewLinda : ILinda {
	public Task<IEnumerable<object[]>> QueryAll();
}