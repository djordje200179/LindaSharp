class LindaDisposedError extends Error {
	public constructor() {
		super("Server Linda was disposed");

		this.name = this.constructor.name;
	}
}

class RemoteLinda {
	private baseUrl: string;
	private actionsUrl: string;
	private healthUrl: string;

	public constructor(host: string, port: number) {
		this.baseUrl = `http://${host}:${port}/`;
		this.actionsUrl = this.baseUrl + "actions/";
		this.healthUrl = this.baseUrl + "health/";
	}

	private async sendTextRequest(method: string, path: string, data: any, contentType: string): Promise<Response> {
		const url = this.actionsUrl + path;
		const headers = {
			"Content-type": contentType
		};
		const response = await fetch(url, {
			method: method,
			headers: headers,
			body: data
		});

		if (response.status == 500)
			throw new LindaDisposedError();

		return response;
	}

	private sendJsonRequest(method: string, path: string, data: any): Promise<Response> {
		return this.sendTextRequest(method, path, JSON.stringify(data), "application/json");
	}

	private async waitTuple(tuplePattern: any[], del: boolean): Promise<any[]> {
		const method = del ? "DELETE" : "GET";
		const path = del ? "rd" : "in";
		const response = await this.sendJsonRequest(method, path, tuplePattern);
		return await response.json();
	}

	private async tryGetTuple(tuplePattern: any[], del: boolean): Promise<any[] | null> {
		const method = del ? "DELETE" : "GET";
		const path = del ? "rdp" : "inp";
		const response = await this.sendJsonRequest(method, path, tuplePattern);
		return await response.json();
	}

	public async out(tuple: any[]): Promise<undefined> {
		await this.sendJsonRequest("POST", "out", tuple);
	}

	public in(tuplePattern: any[]): Promise<any[]> {
		return this.waitTuple(tuplePattern, true);
	}

	public rd(tuplePattern: any[]): Promise<any[]> {
		return this.waitTuple(tuplePattern, false);
	}

	public inp(tuplePattern: any[]): Promise<any[] | null> {
		return this.tryGetTuple(tuplePattern, true);
	}

	public rdp(tuplePattern: any[]): Promise<any[] | null> {
		return this.tryGetTuple(tuplePattern, false);
	}

	public async evalRegister(key: string, ironpythonCode: string): Promise<undefined> {
		await this.sendTextRequest("PUT", `eval/${key}`, ironpythonCode, "text/ironpython");
	}

	public async evalInvoke(key: string, parameter: any = null): Promise<undefined> {
		await this.sendJsonRequest("POST", `eval/${key}`, parameter);
	}

	public async eval(ironpythonCode: string): Promise<undefined> {
		await this.sendTextRequest("POST", `eval`, ironpythonCode, "text/ironpython");
	}

	public async isHealthy(): Promise<boolean> {
		const url = this.healthUrl + "ping";

		try {
			const response = await fetch(url);
			return response.ok;
		} catch {
			return false;
		}
	}
}